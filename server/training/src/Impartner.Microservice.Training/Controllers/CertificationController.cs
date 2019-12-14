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
	[Route("api/ms/v1/certification"), Authorize(PolicyNames.TenantId)]
	[Consumes("application/json"), Produces("application/json")]
	public class CertificationController : ControllerBase
	{
		public const string CollectionName = "certification";
		private readonly IMongoRepository _repository;

		public CertificationController(IMongoRepository repository)
		{
			_repository = repository;
		}

		[ProducesApiResult(HttpStatusCode.Created, typeof(Certification), "Creates a new certification in the database")]
		[ProducesApiResult(HttpStatusCode.BadRequest, description: "Certification request body is invalid")]
		[ProducesApiResult(HttpStatusCode.Forbidden, description: "MongoDB failed to save the new item; Can be resubmitted")]
		[HttpPost, Authorize(PolicyNames.IsAdmin)]
		public async Task<ApiResult<Certification>> AddNewItem([FromBody] Certification data)
		{
			var user = User.ToUserInfo();
			data.CreatedBy = user;
			data.UpdatedBy = user;

			var createdCertification = await _repository.SaveAsync(CollectionName, data);

			if (createdCertification == null)
			{
				throw new HttpConflictException
				(
					new ApiError
					(
						nameof(Conflict),
						$"Unable to create the certification: {{{nameof(data)}}}",
						additionalData: new Dictionary<string, Certification> { { nameof(data), data } }
					)
				);
			}

			return Created($"{HttpContext.GetRequestUrl()}/{createdCertification.Id}", createdCertification);
		}

		[ProducesApiResult(HttpStatusCode.OK, typeof(DeleteResult), "Deletes a single record with the given id from the database")]
		[ProducesApiResult(HttpStatusCode.BadRequest, description: "Certification ID is invalid")]
		[HttpDelete("{id}"), Authorize(PolicyNames.IsAdmin)]
		public async Task<ApiResult<DeleteResult>> Delete([FromRoute] string id)
		{
			var objectId = ParseCertId(id);
			var deleteCourseResult = await _repository.DeleteManyAsync<Course>(CourseController.CollectionName, x => x.CertificationId == id);
			return await _repository.DeleteAsync<Certification>(CollectionName, x => x.Id == objectId);
		}

		[ProducesApiResult(HttpStatusCode.OK, typeof(List<Certification>), "Gets all certifications; Can be restricted by `skip` and `take` query parameters; Defaults: `skip` = 0, `take` = 100")]
		[HttpGet]
		public async Task<ApiResult<List<Certification>>> Get([FromQuery] int skip = 0, [FromQuery] int take = 100)
		{
			var findFluent = _repository.Find<Certification>(CollectionName, new BsonDocument());
			var totalCount = (int)await findFluent.CountDocumentsAsync();

			HttpContext.AddPaginationHeaders(skip, take, totalCount);

			return await findFluent
				.Skip(skip)
				.Limit(take)
				.ToListAsync();
		}

		[ProducesApiResult(HttpStatusCode.OK, typeof(List<Certification>), "Gets all courses associated with a certification; Can be restricted by `skip` and `take` query parameters; Defaults: `skip` = 0, `take` = 100")]
		[HttpGet("{id}/courses")]
		public async Task<ApiResult<List<Course>>> GetCoursesByCertificationId([FromRoute] string id, [FromQuery] int skip = 0, [FromQuery] int take = 100)
		{
			var findFluent = _repository.Find<Course>(CourseController.CollectionName, c => c.CertificationId == id);

			var totalCount = (int)await findFluent.CountDocumentsAsync();

			HttpContext.AddPaginationHeaders(skip, take, totalCount);

			return await findFluent
				.Skip(skip)
				.Limit(take)
				.ToListAsync();
		}

		[ProducesApiResult(HttpStatusCode.OK, typeof(Certification), "Gets the certification by the provided id")]
		[ProducesApiResult(HttpStatusCode.BadRequest, description: "Certification ID is invalid")]
		[ProducesApiResult(HttpStatusCode.NotFound, description: "Certification with the given id does not exist")]
		[HttpGet("{id}")]
		public async Task<ApiResult<Certification>> GetById([FromRoute] string id)
		{
			var objectId = ParseCertId(id);
			var result = await FindCertificationById(objectId);
			if (result == null)
			{
				throw new HttpNotFoundException
				(
					new ApiError
					(
						nameof(NotFound),
						$"Could not find {nameof(Certification)} by $id",
						additionalData: new Dictionary<string, string> { { "id", id } }
					)
				);
			}
			return result;
		}

		[ProducesApiResult(HttpStatusCode.OK, typeof(Certification), "Updates a certification based on the JSON patch values provided")]
		[ProducesApiResult(HttpStatusCode.BadRequest, description: "Certification json patch model was invalid and could not be processed")]
		[ProducesApiResult(HttpStatusCode.NotFound, description: "Certification with the given id does not exist")]
		[HttpPatch("{id}"), Authorize(PolicyNames.IsAdmin)]
		public async Task<ApiResult<Certification>> Patch([FromRoute] string id, [FromBody] JsonPatchDocument<Certification> data)
		{
			var objectId = ParseCertId(id);
			var cert = await FindCertificationById(objectId);
			if (cert == null)
			{
				throw new HttpNotFoundException(new ApiError(
					nameof(NotFound),
					"Could not find {id}",
					additionalData: new Dictionary<string, object> { { "id", id } }));
			}

			var previous = cert.ShallowCopy();
			this.ApplyPatch(data, cert);

			ValidateChanges(previous, cert);
			cert.UpdatedAt = DateTime.UtcNow;
			cert.UpdatedBy = User.ToUserInfo();
			var result = await _repository.UpdateAsync(CollectionName, x => x.Id == objectId, cert);
			return cert;
		}

		#region CertificationStatus

		[ProducesApiResult(HttpStatusCode.OK, typeof(CourseStatus), "Creates a new status for a certification")]
		[ProducesApiResult(HttpStatusCode.BadRequest, description: "Certification ID is invalid")]
		[ProducesApiResult(HttpStatusCode.NotFound, description: "Certification with the given id does not exist")]
		[HttpPost("{id}/status")]
		public async Task<ApiResult<CertificationStatus>> CreateStatus([FromRoute] string id)
		{
			var certId = ParseCertId(id);
			var userId = User.ToUserId();
			var results = await _repository.FindAsync<CertificationStatus>(CertificationStatusController.CollectionName,
				x => x.CertificationId == certId && !x.IsComplete && x.CreatedBy.UserId == userId);
			if (results.Any())
			{
				throw new HttpBadRequestException(new ApiError(nameof(BadRequest), "A status already exists for this certification."));
			}

			var cert = await FindCertificationById(certId);
			AssertCertificationIsActive(cert);
			var courses = await FindCoursesByCertId(id);
			AssertHasAnActiveCourse(courses);
			var status = new CertificationStatus(cert, courses, User.ToUserInfo());
			return await _repository.SaveAsync(CertificationStatusController.CollectionName, status);
		}

		[ProducesApiResult(HttpStatusCode.OK, typeof(CourseStatus), "Gets the current status of a certification")]
		[ProducesApiResult(HttpStatusCode.BadRequest, description: "Certification ID is invalid")]
		[ProducesApiResult(HttpStatusCode.NotFound, description: "Certification with the given id does not exist")]
		[HttpGet("{id}/status")]
		public async Task<ApiResult<List<CertificationStatus>>> GetStatus([FromRoute] string id)
		{
			var certId = ParseCertId(id);
			var userId = User.ToUserId();
			var results = await _repository.FindAsync<CertificationStatus>(CertificationStatusController.CollectionName,
				x => x.CertificationId == certId && x.CreatedBy.UserId == userId);
			return results.ToList().OrderBy(x => x.CreatedAt).ToList();
		}

		#endregion

		#region Private methods

		private static ObjectId ParseCertId(string id)
		{
			if (!ObjectId.TryParse(id, out var objectId))
			{
				throw new HttpBadRequestException(new ApiError(
					nameof(BadRequest),
					$"Id was not a valid MongoDB id: {{{nameof(id)}}}",
					additionalData: new Dictionary<string, string> { { nameof(id), id } }));
			}

			return objectId;
		}

		private async Task<Certification> FindCertificationById(ObjectId certId)
		{
			var results = await _repository.FindAsync<Certification>(CollectionName, x => x.Id == certId);
			return results.SingleOrDefault();
		}

		private async Task<List<Course>> FindCoursesByCertId(string certId)
		{
			var results = await _repository.FindAsync<Course>(CourseController.CollectionName, x => x.CertificationId == certId && x.IsActive);
			return results.ToList();
		}

		private void AssertCertificationIsActive(Certification cert)
		{
			if (!cert.IsActive)
			{
				throw new HttpStatusCodeException(HttpStatusCode.MethodNotAllowed, new ApiError
				(
					nameof(BadRequest),
					$"May not start a certification when it is set to inactive"
				));
			}
		}

		private void AssertHasAnActiveCourse(List<Course> courses)
		{
			if (!courses.Any(x => x.IsActive))
			{
				throw new HttpStatusCodeException(HttpStatusCode.MethodNotAllowed, new ApiError
				(
					nameof(BadRequest),
					$"May not start a certification without an active course"
				));
			}
		}

		private void ValidateChanges(Certification previous, Certification current)
		{
			if (IsStayingInactiveOrBeingDeactivated(previous, current))
				return;

			AssertIsNotBeingEditedWhileActive(previous, current);
			AssertCanBeActivated(previous, current);
		}

		private bool IsStayingInactiveOrBeingDeactivated(Certification previous, Certification current)
		{
			return !previous.IsActive && !current.IsActive || previous.IsActive && !current.IsActive;
		}

		private void AssertIsNotBeingEditedWhileActive(Certification previous, Certification current)
		{
			if (previous.IsActive && current.IsActive)
			{
				throw new HttpBadRequestException(new ApiError(
					nameof(BadRequest),
					"A certification must be inactive to edit"));
			}
		}

		private void AssertCanBeActivated(Certification previous, Certification current)
		{
			if (!previous.IsActive && current.IsActive)
			{
				if (!current.Courses.Any(x => x.IsActive))
				{
					throw new HttpBadRequestException(new ApiError(
						nameof(BadRequest),
						"To be activated a certification must have at least one active course"));
				}

				current.CertificationVersion++;
			}
		}

		#endregion
	}
}
