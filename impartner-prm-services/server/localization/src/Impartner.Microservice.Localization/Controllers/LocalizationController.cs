using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Impartner.Microservice.Common.Attributes;
using Impartner.Microservice.Common.Authorization;
using Impartner.Microservice.Common.Exceptions.Http;
using Impartner.Microservice.Common.Extensions;
using Impartner.Microservice.Common.Models.Responses;
using Impartner.Microservice.Common.Mongo.Repositories;
using Impartner.Microservice.Localization.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using LocalizationModel = Impartner.Microservice.Localization.Models.LocalizationModel.V1;

namespace Impartner.Microservice.Localization.Controllers
{
	[ApiController]
	[Route("api/ms/v1/localization"), Authorize(PolicyNames.TenantId)]
	[Consumes( "application/json"), Produces("application/json")]
	public class LocalizationController : ControllerBase
	{
		#region Fields

		public const string CollectionName = "data";

		private readonly IMongoRepository _repository;

		#endregion

		#region Constructors

		public LocalizationController(IMongoRepository repository)
		{
			_repository = repository;
		}

		#endregion

		#region Public Methods

		[ProducesApiResult(HttpStatusCode.Created, typeof(LocalizationModel), "Creates a new localization entry in the database")]
		[ProducesApiResult(HttpStatusCode.BadRequest, description: "Localization model request body is invalid")]
		[ProducesApiResult(HttpStatusCode.Forbidden, description: "MongoDB failed to save the new item; Can be resubmitted")]
		[HttpPost, Authorize(PolicyNames.IsAdmin)]
		public async Task<ApiResult<LocalizationModel>> AddNewItem([FromBody] LocalizationModel document)
		{
			var results = await _repository.FindAsync<LocalizationModel>(CollectionName, x => x.ObjectId == document.ObjectId && x.ObjectName == document.ObjectName);
			var result = results.SingleOrDefault();
			if (result != null)
			{
				throw new HttpForbidException
				(
					new ApiError
					(
						nameof(Forbid),
						$"Item with {nameof(document.ObjectId)}{document.ObjectId} and {nameof(document.ObjectName)}{document.ObjectName} already exists in database"
					)
				);
			}

			var createdResult = await _repository.SaveAsync(CollectionName, document);

			if (createdResult == null)
			{
				throw new HttpConflictException
				(
					new ApiError(
						nameof(Conflict),
						$"Unable to create the localization: {{{nameof(document)}}}",
						additionalData: new Dictionary<string, LocalizationModel> { {nameof(document), document}}) 
				);
			}

			return Created($"{HttpContext.GetRequestUrl()}?objectId={createdResult.ObjectId}&objectName={createdResult.ObjectName}", createdResult);
		}

		[ProducesApiResult(HttpStatusCode.OK, typeof(DeleteResult), "Deletes a single record with the given id from the database.")]
		[ProducesApiResult(HttpStatusCode.BadRequest, description: "Localization model ID was invalid")]
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
						additionalData: new Dictionary<string, string> {{nameof(id), id}}
					)
				);
			}
			return await _repository.DeleteAsync<LocalizationModel>(CollectionName, x => x.Id == objectId);
		}

		[ProducesApiResult ( HttpStatusCode.OK, typeof(LocalizationModel), "Retrieves a localization entry by " + nameof(LocalizationModel.ObjectName) + " and " + nameof(LocalizationModel.ObjectId))]
		[ProducesApiResult ( HttpStatusCode.NotFound, description: "Localization model could not be found by the provided " + nameof(LocalizationModel.ObjectName) + " and " + nameof(LocalizationModel.ObjectId))]
		[HttpGet]
		public async Task<ApiResult<LocalizationModel>> Get([FromQuery] string objectId, [FromQuery] string objectName)
		{
			var results = await _repository.FindAsync<LocalizationModel>(CollectionName, x => x.ObjectId == objectId && x.ObjectName == objectName);
			var result = results.SingleOrDefault();
			if (result == null)
			{
				throw new HttpNotFoundException
				(
					new ApiError
					(
						nameof(NotFound),
						$"Could not find {nameof(LocalizationModel)} by {nameof(LocalizationModel.ObjectId)}({{{nameof(objectId)}}}) " +
						$"and {nameof(LocalizationModel.ObjectName)}({{{nameof(objectName)}}})",
						additionalData: new Dictionary<string, string> {{ nameof(objectId), objectId}, {nameof(objectName), objectName}}
					)
				);
			}
			return result;
		}

		[ProducesApiResult(HttpStatusCode.OK,typeof(LocalizationModel),"Retrieves a localization entry by " + nameof(LocalizationModel.Id))]
		[ProducesApiResult(HttpStatusCode.BadRequest, description: "Localization model ID is invalid")]
		[ProducesApiResult(HttpStatusCode.NotFound, description: "Localization model with the given id does not exist")]
		[HttpGet("{id}")]
		public async Task<ApiResult<LocalizationModel>> GetById([FromRoute] string id)
		{
			if (!ObjectId.TryParse(id, out var objectId))
			{
				throw new HttpBadRequestException
				(
					new ApiError
					(
						nameof(BadRequest),
						$"Id was not a valid MongoDB id: {{{nameof(id)}}}",
						additionalData: new Dictionary<string, string> {{nameof(id), id}}
					)
				);
			}

			var result = await FindById(objectId);
			if (result == null)
			{
				throw new HttpNotFoundException
				(
					new ApiError
					(
						nameof(NotFound),
						$"Could not find {nameof(LocalizationModel)} by {nameof(LocalizationModel.Id)}({{{nameof(id)}}})",
						additionalData: new Dictionary<string, string> { { nameof(id), id }}
					)
				);
			}
			return result;
		}

		[ProducesApiResult(HttpStatusCode.OK, typeof(LocalizationModel), "Updates a localization model based on the JSON patch values provided")]
		[ProducesApiResult(HttpStatusCode.BadRequest, description: "Localization model json patch model was invalid and could not be processed")]
		[ProducesApiResult(HttpStatusCode.NotFound, description: "Localization model with the given id does not exist")]
		[HttpPatch("{id}"), Authorize(PolicyNames.IsAdmin)]
		public async Task<ApiResult<LocalizationModel>> Patch(string id, [FromBody] JsonPatchDocument<LocalizationModel> data)
		{
			var objectId = ObjectId.Parse(id);
			var localizationModel = await FindById(objectId);
			if (localizationModel == null)
			{
				throw new HttpNotFoundException
				(
					new ApiError
					(
						nameof(NotFound),
						$"Could not find {nameof(LocalizationModel)} by {nameof(LocalizationModel.Id)}({{{nameof(id)}}})",
						additionalData: new Dictionary<string, string> { { nameof(id), id }}
					)
				);
			}

			this.ApplyPatch(data, localizationModel);

			var _ = await _repository.UpdateAsync(CollectionName, x => x.Id == objectId, localizationModel);
			return localizationModel;
		}

		[ProducesApiResult(HttpStatusCode.Created, typeof(LocalizationModel), "Creates a copy of the localization model with a new object id if it doesn't already exist")]
		[ProducesApiResult(HttpStatusCode.NotModified, description: "Localization model was found but a the localization already exists")]
		[ProducesApiResult(HttpStatusCode.NotFound, description: "Localization model with the given " + nameof(LocalizationModel.ObjectName) + " and " + nameof(LocalizationModel.ObjectId) + " does not exist")]
		[HttpPut, Authorize(PolicyNames.IsAdmin)]
		public async Task<ApiResult<LocalizationModel>> Upsert([FromBody] LocalizationCopy localizationCopy)
		{
			var existing = await FindExistingLocalization(localizationCopy, localizationCopy.ObjectName);
			if (existing != null)
			{
				return Ok(existing);
			}

			var model = await FindLocalization(localizationCopy.ObjectId, localizationCopy.ObjectName);
			if (model == null)
			{
				throw new HttpNotFoundException
				(
					new ApiError
					(
						nameof(NotFound),
						$"Could not find {nameof(LocalizationModel)} by {nameof(LocalizationModel.ObjectId)}({{objectId}}) " +
						$"and {nameof(LocalizationModel.ObjectName)}({{objectName}})",
						additionalData: new Dictionary<string, string> {{ "objectId", localizationCopy.ObjectId }, { "objectName", localizationCopy.ObjectName }}
					)
				);
			}

			var copy = CopyExistingLocalization(model, localizationCopy.NewObjectId);
			return await AddNewItem(copy);
		}

		#endregion

		#region Private Methods

		private static LocalizationModel CopyExistingLocalization(LocalizationModel localizationModel, string newObjectId)
		{
			var copy = BsonSerializer.Deserialize<LocalizationModel>(localizationModel.ToBsonDocument());
			copy.Id = ObjectId.GenerateNewId();
			copy.ObjectId = newObjectId;
			return copy;
		}

		private async Task<LocalizationModel> FindById(ObjectId objectId)
		{
			var results = await _repository.FindAsync<LocalizationModel>(CollectionName, x => x.Id == objectId);
			return results.SingleOrDefault();
		}

		private async Task<LocalizationModel> FindExistingLocalization(LocalizationCopy localizationCopy, string objectName)
		{
			var results = await _repository.FindAsync<LocalizationModel>(CollectionName, x => x.ObjectId == localizationCopy.NewObjectId && x.ObjectName == objectName);
			return results.SingleOrDefault();
		}

		private async Task<LocalizationModel> FindLocalization(string objectId, string objectName)
		{
			var results = await _repository.FindAsync<LocalizationModel>(CollectionName, x => x.ObjectId == objectId && x.ObjectName == objectName);
			return results.SingleOrDefault();
		}

		#endregion
	}
}
