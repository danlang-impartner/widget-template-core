using Impartner.Microservice.Common.Attributes;
using Impartner.Microservice.Common.Authorization;
using Impartner.Microservice.Common.Exceptions.Http;
using Impartner.Microservice.Common.Extensions;
using Impartner.Microservice.Common.Models.Responses;
using Impartner.Microservice.Common.Mongo.Repositories;
using Impartner.Microservice.Training.Models;
using Impartner.Microservice.Training.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Impartner.Microservice.Training.Controllers
{
	[ApiController]
	[Authorize(PolicyNames.TenantId)]
	[Route("api/ms/v1/certificationStatus")]
	public class CertificationStatusController : ControllerBase
	{
		public const string CollectionName = "certificationStatus";
		private readonly IMongoRepository _repository;
		private readonly ICertificationStatusUpdator _certStatusUpdator;
		private readonly ICourseCompletionHandler _courseCompletionHandler;
		private readonly ILogger<CertificationStatusController> _logger;

		public CertificationStatusController(IMongoRepository repository, ICertificationStatusUpdator certStatusUpdator,
			ICourseCompletionHandler courseCompletionHandler, ILogger<CertificationStatusController> logger)
		{
			_repository = repository;
			_certStatusUpdator = certStatusUpdator;
			_courseCompletionHandler = courseCompletionHandler;
			_logger = logger;
		}

		[ProducesApiResult(HttpStatusCode.OK, typeof(CourseStatus), "Restarts a quiz from a current certification status' course status")]
		[ProducesApiResult(HttpStatusCode.NotFound, description: "Certification status with the given id does not exist")]
		[ProducesApiResult(HttpStatusCode.BadRequest, description: "Id is invalid")]
		[HttpGet("{id}/course/{courseId}/quiz/{quizId}/restart")]
		public async Task<ApiResult<CertificationStatus>> RestartQuiz([FromRoute] string id, [FromRoute] string courseId, [FromRoute] string quizId)
		{
			var certId = ParseObjectId(id, nameof(id));
			var certStatus = await FindCertStatusById(certId);
			var courseObjectId = ParseObjectId(courseId, nameof(courseId));
			var courseStatus = GetCourseStatus(certStatus, courseObjectId);
			var quizObjectId = ParseObjectId(quizId, nameof(quizId));
			var quiz = GetQuiz(courseStatus, quizObjectId);
			courseStatus.RestartQuiz(quiz);
			var result = await _repository.UpdateAsync(CollectionName, x => x.Id == certId, certStatus);
			return certStatus;
		}

		[ProducesApiResult(HttpStatusCode.OK, typeof(CourseStatus), "Updates the state of the current certification progress")]
		[ProducesApiResult(HttpStatusCode.NotFound, description: "Certification status with the given id does not exist")]
		[ProducesApiResult(HttpStatusCode.MethodNotAllowed, description: "Certification status may not be modified")]
		[ProducesApiResult(HttpStatusCode.BadRequest, description: "Id is invalid")]
		[HttpPatch("{id}")]
		public async Task<ActionResult<CertificationStatus>> Patch([FromRoute] string id, [FromBody] JsonPatchDocument<CertificationStatus> data)
		{
			var certId = ParseObjectId(id, nameof(id));
			var certStatus = await FindCertStatusById(certId);
			AssertCertStatusIsNotCompleted(certStatus);

			var previousPassedCourseIds = certStatus.Courses.Where(c => c.HasPassed).Select(c => c.CourseId).ToList();
			this.ApplyPatch(data, certStatus);
			var updatedCertStatus = _certStatusUpdator.Update(certStatus);

			var result = await _repository.UpdateAsync(CollectionName, x => x.Id == certId, updatedCertStatus);
			var currentPassedCourseIds = updatedCertStatus.Courses.Where(c => c.HasPassed).Select(c => c.CourseId);
			var justPassedId = currentPassedCourseIds.Except(previousPassedCourseIds).SingleOrDefault();
			_logger.LogDebug($"Course just completed: {justPassedId}");
			if (justPassedId != ObjectId.Empty)
			{
				await _courseCompletionHandler.CompleteCourse(updatedCertStatus, updatedCertStatus.Courses.Single(c => c.CourseId == justPassedId));
			}
			return updatedCertStatus;
		}

		#region Private methods

		private async Task<CertificationStatus> FindCertStatusById(ObjectId certId)
		{
			var results = await _repository.FindAsync<CertificationStatus>(CollectionName, x => x.Id == certId);
			var status = results.SingleOrDefault();
			if (status == null)
			{
				throw new HttpNotFoundException(new ApiError(
					nameof(NotFound),
					$"Could not find {nameof(CourseStatus)} by $id",
					additionalData: new Dictionary<string, string> { { "id", certId.ToString() } }));
			}
			return status;
		}

		private CourseStatus GetCourseStatus(CertificationStatus certStatus, ObjectId courseId)
		{
			var courseStatus = certStatus.Courses.SingleOrDefault(x => x.CourseId == courseId);
			if (courseStatus == null)
			{
				throw new HttpNotFoundException(new ApiError(
					nameof(NotFound),
					$"Could not find {nameof(Course)} by $id",
					additionalData: new Dictionary<string, string> { { "id", courseId.ToString() } }));
			}
			return courseStatus;
		}

		private CertStatusQuiz GetQuiz(CourseStatus courseStatus, ObjectId quizId)
		{
			var quiz = courseStatus.Quizzes.SingleOrDefault(x => x.QuizId == quizId);
			if (quiz == null)
			{
				throw new HttpNotFoundException(new ApiError(
					nameof(NotFound),
					$"Could not find {nameof(Quiz)} by $id",
					additionalData: new Dictionary<string, string> { { "id", quizId.ToString() } }));
			}
			return quiz;
		}

		private static ObjectId ParseObjectId(string id, string idName)
		{
			if (!ObjectId.TryParse(id, out var objectId))
			{
				throw new HttpBadRequestException
				(
					new ApiError
					(
						nameof(BadRequest),
						$"{idName} was not a valid MongoDB id: {{{idName}}}",
						additionalData: new Dictionary<string, string> { { idName, id } }
					)
				);
			}

			return objectId;
		}

		private static void AssertCertStatusIsNotCompleted(CertificationStatus status)
		{
			if (status.IsComplete)
			{
				throw new HttpStatusCodeException
				(
					HttpStatusCode.MethodNotAllowed,
					new ApiError
					(
						nameof(BadRequest),
						"May not modify a completed certification status"
					)
				);
			}
		}

		#endregion
	}
}
