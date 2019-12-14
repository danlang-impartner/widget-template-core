using Impartner.Microservice.Common.Attributes;
using Impartner.Microservice.Common.Authorization;
using Impartner.Microservice.Common.Exceptions.Http;
using Impartner.Microservice.Common.Extensions;
using Impartner.Microservice.Common.Models.Responses;
using Impartner.Microservice.Common.Mongo.Repositories;
using Impartner.Microservice.Training.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Impartner.Microservice.Training.Controllers
{
	[ApiController]
	[Authorize(PolicyNames.TenantId)]
	[Route("api/ms/v1/course")]
	public class CourseController : ControllerBase
	{
		#region Fields

		public const string CollectionName = "course";
		private readonly IMongoRepository _repository;

		#endregion

		#region Constructors

		public CourseController(IMongoRepository repository)
		{
			_repository = repository;
		}

		#endregion

		#region Public Methods

		[ProducesApiResult(HttpStatusCode.Created, typeof(Course), "Creates a new course in the database")]
		[ProducesApiResult(HttpStatusCode.BadRequest, description: "Course request body is invalid")]
		[ProducesApiResult(HttpStatusCode.Forbidden, description: "MongoDB failed to save the new item; Can be resubmitted")]
		[HttpPost, Authorize(PolicyNames.IsAdmin)]
		public async Task<ApiResult<Course>> AddNewItem([FromBody] Course data)
		{
			var user = User.ToUserInfo();
			data.CreatedBy = user;
			data.UpdatedBy = user;

			var result = await _repository.SaveAsync(CollectionName, data);

			if (result == null)
			{
				throw new HttpConflictException
				(
					new ApiError(
						nameof(Conflict),
						$"Unable to create the form: {{{nameof(data)}}}",
						additionalData: new Dictionary<string, Course> { { nameof(data), data } })
				);
			}

			var certResult = await CreateDenormalizedData(data);

			return Created($"{HttpContext.GetRequestUrl()}/{result.Id}", result);
		}

		[ProducesApiResult(HttpStatusCode.OK, typeof(DeleteResult), "Deletes a single record with the given id from the database")]
		[ProducesApiResult(HttpStatusCode.BadRequest, description: "Course ID is invalid")]
		[HttpDelete("{id}"), Authorize(PolicyNames.IsAdmin)]
		public async Task<ApiResult<DeleteResult>> Delete([FromRoute] string id)
		{
			if (!ObjectId.TryParse(id, out var objectId))
			{
				throw new HttpBadRequestException
				(
					new ApiError
					(
						nameof(BadRequest),
						$"Id was not a valid MongoDB id: {{{nameof(id)}}}",
						additionalData: new Dictionary<string, string> { { nameof(id), id } }
					)
				);
			}
			var certResult = await DeleteDenormalizedData(objectId);
			var courseResult = await _repository.DeleteAsync<Course>(CollectionName, x => x.Id == objectId);
			return courseResult;
		}

		[ProducesApiResult(HttpStatusCode.OK, typeof(List<Course>), "Gets all courses; Can be restricted by `skip` and `take` query parameters; Defaults: `skip` = 0, `take` = 100")]
		[HttpGet]
		public async Task<ApiResult<List<Course>>> Get([FromQuery] int skip = 0, [FromQuery] int take = 100)
		{
			var findFluent = _repository.Find<Course>(CollectionName, new BsonDocument());
			var totalCount = (int)await findFluent.CountDocumentsAsync();

			HttpContext.AddPaginationHeaders(skip, take, totalCount);

			return await findFluent
				.Skip(skip)
				.Limit(take)
				.ToListAsync();
		}

		[ProducesApiResult(HttpStatusCode.OK, typeof(Course), "Gets the course by the provided id")]
		[ProducesApiResult(HttpStatusCode.BadRequest, description: "Course ID is invalid")]
		[ProducesApiResult(HttpStatusCode.NotFound, description: "Course with the given id does not exist")]
		[HttpGet("{id}")]
		public async Task<ApiResult<Course>> GetById([FromRoute] string id)
		{
			var objectId = ParseCourseId(id);
			var result = await FindCourseById(objectId);
			return result;
		}

		[ProducesApiResult(HttpStatusCode.OK, typeof(Course), "Updates a course based on the JSON patch values provided")]
		[ProducesApiResult(HttpStatusCode.BadRequest, description: "Course json patch model was invalid and could not be processed")]
		[ProducesApiResult(HttpStatusCode.NotFound, description: "Course with the given id does not exist")]
		[HttpPatch("{id}"), Authorize(PolicyNames.IsAdmin)]
		public async Task<ApiResult<Course>> Patch([FromRoute] string id, [FromBody] JsonPatchDocument<Course> data)
		{
			var objectId = ParseCourseId(id);
			var course = await FindCourseById(objectId);
			if (course == null)
			{
				throw new HttpNotFoundException(new ApiError(
					nameof(NotFound),
					"Could not find {id}",
					additionalData: new Dictionary<string, object> { { "id", id } }));
			}

			var previous = course.ShallowCopy();
			this.ApplyPatch(data, course);

			ValidateChanges(previous, course);
			course.UpdatedAt = DateTime.UtcNow;
			course.UpdatedBy = User.ToUserInfo();
			var result = await _repository.UpdateAsync(CollectionName, x => x.Id == objectId, course);

			if (previous.IsActive != course.IsActive || previous.Name != course.Name)
			{
				var certResult = await UpdateDenormalizedData(previous, course);
			}

			return course;
		}

		#endregion

		#region Private Methods

		private static ObjectId ParseCourseId(string id)
		{
			if (!ObjectId.TryParse(id, out var objectId))
			{
				throw new HttpBadRequestException
				(
					new ApiError
					(
						nameof(BadRequest),
						$"Id was not a valid MongoDB id: {{{nameof(id)}}}",
						additionalData: new Dictionary<string, string> { { nameof(id), id } }
					)
				);
			}

			return objectId;
		}

		private async Task<Course> FindCourseById(ObjectId courseId)
		{
			var results = await _repository.FindAsync<Course>(CollectionName, x => x.Id == courseId);
			var course = results.SingleOrDefault();
			if (course == null)
			{
				throw new HttpNotFoundException
				(
					new ApiError(
						nameof(NotFound),
						"Couldn't find Course with Id {id}",
						additionalData: new Dictionary<string, object> { { "id", courseId } }
					)
				);
			}
			return course;
		}

		private async Task<Certification> FindCertificationById(ObjectId certId)
		{
			var results = await _repository.FindAsync<Certification>(CertificationController.CollectionName, x => x.Id == certId);
			var cert = results.SingleOrDefault();
			if (cert == null)
			{
				throw new HttpNotFoundException
				(
					new ApiError(
						nameof(NotFound),
						"Couldn't find Certification with Id {id}",
						additionalData: new Dictionary<string, object> { { "id", certId } }
					)
				);
			}
			return cert;
		}

		private async Task<ReplaceOneResult> UpdateDenormalizedData(Course original, Course updated)
		{
			var certObjectId = ObjectId.Parse(original.CertificationId);
			var certification = await FindCertificationById(certObjectId);
			certification.Courses.RemoveAll(x => x.Id == original.Id);
			certification.Courses.Add(new DenormalizedCourse(updated));
			return await _repository.UpdateAsync(CertificationController.CollectionName, x => x.Id == certObjectId, certification);
		}

		private async Task<ReplaceOneResult> CreateDenormalizedData(Course course)
		{
			var certId = ObjectId.Parse(course.CertificationId);
			var certification = await FindCertificationById(certId);
			certification.Courses.Add(new DenormalizedCourse(course));
			return await _repository.UpdateAsync(CertificationController.CollectionName, x => x.Id == certId, certification);
		}

		private async Task<ReplaceOneResult> DeleteDenormalizedData(ObjectId courseId)
		{
			var course = await FindCourseById(courseId);

			var certId = ObjectId.Parse(course.CertificationId);
			var cert = await FindCertificationById(certId);
			cert.Courses.RemoveAll(c => c.Id == course.Id);

			return await _repository.UpdateAsync(CertificationController.CollectionName, x => x.Id == certId, cert);
		}

		private void ValidateChanges(Course previous, Course current)
		{
			if(IsStayingInactiveOrBeingDeactivated(previous, current))
				return;

			AssertIsNotBeingEditedWhileActive(previous, current);
			AssertAllQuestionsHaveCorrectAnswers(previous, current);
			AssertHasActiveLessonAndQuiz(previous, current);
		}


		private bool IsStayingInactiveOrBeingDeactivated(Course previous, Course current)
		{
			return !previous.IsActive && !current.IsActive || previous.IsActive && !current.IsActive;
		}

		private void AssertIsNotBeingEditedWhileActive(Course previous, Course current)
		{
			if (previous.IsActive && current.IsActive)
			{
				throw new HttpBadRequestException(new ApiError(
					nameof(BadRequest),
					"A course must be inactive to edit"));
			}
		}

		private void AssertAllQuestionsHaveCorrectAnswers(Course previous, Course current)
		{
			if (current.Quizzes.Any(quiz => quiz.IsActive && !quiz.Questions.All(q => q.CorrectAnswers.Any())))
			{
				throw new HttpBadRequestException(new ApiError(
					nameof(BadRequest),
					"Each question in an active quiz must have at least one correct answer"));
			}
		}

		private void AssertHasActiveLessonAndQuiz(Course previous, Course current)
		{
			if (!previous.IsActive && current.IsActive)
			{
				if (!current.Lessons.Any(l => l.IsActive) || !current.Quizzes.Any(q => q.IsActive))
				{
					throw new HttpBadRequestException(new ApiError(
						nameof(BadRequest),
						"To be activated a course must have at least one active lesson and quiz"));
				}

				current.CourseVersion++;
			}
		}

		#endregion
	}
}
