using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Impartner.Microservice.Common.Attributes;
using Impartner.Microservice.Common.Authorization;
using Impartner.Microservice.Common.Exceptions.Http;
using Impartner.Microservice.Common.Extensions;
using Impartner.Microservice.Common.Models.Responses;
using Impartner.Microservice.Common.Mongo.Repositories;
using Impartner.Microservice.DynamicForms.Models;
using DynamicForm = Impartner.Microservice.DynamicForms.Models.DynamicForm.V1;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Impartner.Microservice.DynamicForms.Controllers
{
	[ApiController]
	[Route("api/ms/v1/form"), Authorize(PolicyNames.TenantId)]
	[Consumes( "application/json"), Produces("application/json")]
	public class FormController : ControllerBase
	{
		#region Fields

		public const string CollectionName = "form";
		private readonly IMongoRepository _repository;

		#endregion

		#region Constructors

		public FormController(IMongoRepository repository)
		{
			_repository = repository;
		}

		#endregion

		#region Public Methods

		[ProducesApiResult(HttpStatusCode.Created, typeof(DynamicForm), "Creates a new dynamic form in the database")]
		[ProducesApiResult(HttpStatusCode.BadRequest, description: "Dynamic form request body is invalid")]
		[ProducesApiResult(HttpStatusCode.Forbidden, description: "MongoDB failed to save the new item; Can be resubmitted")]
		[HttpPost, Authorize(PolicyNames.IsAdmin)]
		public async Task<ApiResult<DynamicForm>> AddNewItem([FromBody] DynamicForm data)
		{
			// TODO - No check for whether the item already exists.
			var createdForm = await _repository.SaveAsync(CollectionName, data);

			if (createdForm == null)
			{
				throw new HttpConflictException
				(
					new ApiError(
						nameof(Conflict),
						$"Unable to create the form: {{{nameof(data)}}}",
						additionalData: new Dictionary<string, DynamicForm> { {nameof(data), data}}) 
				);
			}

			return Created($"{HttpContext.GetRequestUrl()}/{createdForm.Id}", createdForm);
		}

		[ProducesApiResult(HttpStatusCode.OK, typeof(DeleteResult), "Deletes a single record with the given id from the database")]
		[ProducesApiResult(HttpStatusCode.BadRequest, description: "Dynamic form ID is invalid")]
		[HttpDelete("{id}"), Authorize(PolicyNames.IsAdmin)]
		public async Task<ApiResult<DeleteResult>> Delete([FromRoute] string id)
		{
			var objectId = ConvertObjectId(id);
			return await _repository.DeleteAsync<DynamicForm>(CollectionName, x => x.Id == objectId);
		}

		[ProducesApiResult(HttpStatusCode.OK,typeof(List<DynamicForm>),"Gets all dynamic forms; Can be restricted by `skip` and `take` query parameters; Defaults: `skip` = 0, `take` = 100")]
		[HttpGet]
		public async Task<ApiResult<List<DynamicForm>>> Get([FromQuery] int skip = 0, [FromQuery] int take = 100)
		{
			var findFluent = _repository.Find<DynamicForm>(CollectionName, new BsonDocument());
			var totalCount = (int) await findFluent.CountDocumentsAsync();

			HttpContext.AddPaginationHeaders(skip, take, totalCount);

			return await findFluent
					.Skip(skip)
					.Limit(take)
					.ToListAsync();
		}

		[ProducesApiResult(HttpStatusCode.OK, typeof(DynamicForm), "Gets the dynamic form by the provided id")]
		[ProducesApiResult(HttpStatusCode.BadRequest, description: "Dynamic form ID is invalid")]
		[ProducesApiResult(HttpStatusCode.NotFound, description: "Dynamic form with the given id does not exist")]
		[HttpGet("{id}")]
		public async Task<ApiResult<DynamicForm>> GetById([FromRoute] string id)
		{
			var objectId = ConvertObjectId(id);
			var result = await FindFormById(objectId);
			
			return result;
		}

		[ProducesApiResult(HttpStatusCode.OK, typeof(DynamicForm), "Updates a dynamic form based on the JSON patch values provided")]
		[ProducesApiResult(HttpStatusCode.BadRequest, description: "Dynamic form json patch model was invalid and could not be processed")]
		[ProducesApiResult(HttpStatusCode.NotFound, description: "Dynamic form with the given id does not exist")]
		[HttpPatch("{id}"), Authorize(PolicyNames.IsAdmin)]
		public async Task<ApiResult<DynamicForm>> Patch([FromRoute] string id, [FromBody]JsonPatchDocument<DynamicForm> data)
		{
			var objectId = ConvertObjectId(id);
			var form = await FindFormById(objectId);

			this.ApplyPatch(data, form);

			await _repository.UpdateAsync(CollectionName, x => x.Id == objectId, form);
			return form;
		}

		[ProducesApiResult(HttpStatusCode.OK, typeof(DynamicForm), "Connected a dynamic form to a parent widget")]
		[ProducesApiResult(HttpStatusCode.BadRequest, description: "Bad id or bad widget id was provided")]
		[ProducesApiResult(HttpStatusCode.NotFound, description: "Dynamic form with the given id does not exist")]
		[HttpPost("{id}/connect"), Authorize(PolicyNames.IsAdmin)]
		public async Task<ApiResult> ConnectToWidget([FromRoute] string id, [Required, FromQuery] int widgetId)
		{
			var objectId = ConvertObjectId(id);
			var form = await FindFormById(objectId);

			if (form.WidgetId > 0)
			{
				throw new HttpConflictException
				(
					new ApiError(
						nameof(Conflict),
						"Form already has an connected widget id: {widgetId} and cannot be updated to {newWidgetId}.",
						additionalData: new Dictionary<string, int> {{"widgetId", form.WidgetId}, {"newWidgetId", widgetId }}
					)
				);
			}

			if (widgetId <= 0)
			{
				throw new HttpBadRequestException
				(
					new ApiError
					(
						nameof(BadRequest),
						$"Widget Id provided was not valid: {{{nameof(widgetId)}}}",
						additionalData: new Dictionary<string, int> {{nameof(widgetId), widgetId}}
					)
				);
			}

			form.WidgetId = widgetId;
			await _repository.UpdateAsync(CollectionName, x => x.Id == objectId, form);

			return Ok();
		}

		[ProducesApiResult(HttpStatusCode.Created, typeof(DynamicForm), "Creates a copy of the dynamic form with a new revision number if it doesn't already exist")]
		[ProducesApiResult(HttpStatusCode.BadRequest, description: "Dynamic Form ID was invalid")]
		[ProducesApiResult(HttpStatusCode.NotModified, description: "Dynamic form was found but a form with the given revision already exists")]
		[ProducesApiResult(HttpStatusCode.NotFound, description: "Dynamic form with the given id does not exist")]
		[HttpPut, Authorize(PolicyNames.IsAdmin)]
		public async Task<ApiResult<DynamicForm>> Upsert([FromBody] DynamicFormCopy formCopy)
		{
			var objectId = ConvertObjectId(formCopy.Id);
			var form = await FindFormById(objectId);

			var existing = await FindExistingForm(formCopy, form.WidgetId);
			if (existing != null)
			{
				return Ok(existing);
			}

			var copy = CopyExistingForm(form, formCopy.Revision);
			return await AddNewItem(copy);
		}

		#endregion

		#region Private Methods

		private static DynamicForm CopyExistingForm(DynamicForm form, int revision)
		{
			var copy = BsonSerializer.Deserialize<DynamicForm>(form.ToBsonDocument());
			copy.Id = ObjectId.GenerateNewId();
			copy.Revision = revision;
			return copy;
		}

		/// <summary>
		/// Validates the object id is a valid <see cref="ObjectId"/>.
		/// </summary>
		/// <param name="id">The id being validated.</param>
		/// <returns>The converted id</returns>
		/// <exception cref="HttpBadRequestException">Thrown if the id is malformed. Returns a 400 HTTP Response.</exception>
		private static ObjectId ConvertObjectId(string id)
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

			return objectId;
		}


		private async Task<DynamicForm> FindExistingForm(DynamicFormCopy formCopy, int widgetId)
		{
			var results = await _repository.FindAsync<DynamicForm>(CollectionName,
				x => x.WidgetId == widgetId && x.Revision == formCopy.Revision);
			return results.SingleOrDefault();
		}

		/// <summary>
		/// Finds a form by the given id.
		/// </summary>
		/// <param name="objectId">The id of the dynamic form to retrieve.</param>
		/// <returns>The dynamic form with the given id.</returns>
		/// <exception cref="HttpNotFoundException">Thrown if form cannot be found; Will result in a 404 HTTP Response.</exception>
		private async Task<DynamicForm> FindFormById(ObjectId objectId)
		{
			var results = await _repository.FindAsync<DynamicForm>(CollectionName, x => x.Id == objectId);

			return results.SingleOrDefault() ?? throw new HttpNotFoundException
				(
					new ApiError(
						nameof(NotFound),
						"Could not find dynamic form with id: {id}",
						additionalData: new Dictionary<string, object> {{"id", objectId}}
					)
				);
		}

		#endregion
	}
}
