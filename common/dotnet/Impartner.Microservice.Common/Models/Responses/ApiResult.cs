using System.Linq;
using System.Net;
using Impartner.Common.Api;
using Impartner.Microservice.Common.Exceptions.Http;
using Impartner.Microservice.Common.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json;

namespace Impartner.Microservice.Common.Models.Responses
{
	/// <summary>
	/// Result of an API request.
	/// </summary>
	public class ApiResult : IConvertToActionResult, IApiResult
	{
		#region Properties

		/// <summary>
		/// Collection of errors associated with the API.
		/// </summary>
		public ApiError[] Errors { get; set; } = new ApiError[0];

		/// <summary>
		/// A top level summary message of the status of this result.
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string Message { get; set; }

		/// <summary>
		/// Dummy data property. This value will always be null, and will provide a consistent experience for users of the API
		/// regardless of the type of ApiResult returned.
		/// </summary>
		public object Data { get; } = default;

		/// <summary>
		/// The action result that will be processed as the response to the request.
		/// </summary>
		[JsonIgnore]
		protected IActionResult ActionResult { get; set; }
			= new StatusCodeResult((int) HttpStatusCode.OK);

		/// <summary>
		/// Wrapper property that fulfills the requirements of IApiResults, but also allows overriding the Errors property.
		/// </summary>
		IApiError[] IApiResult.Errors
		{
			get => Errors?.OfType<IApiError>().ToArray() ?? new IApiError[0];
			set => Errors = value?.OfType<ApiError>().ToArray() ?? new ApiError[0];
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Implicit conversion operator that will convert be able to convert an ActionResult to an ApiResult.
		///
		/// This specifically adds the ability for a controller to be written utilizing all the baked in ActionResult functions,
		/// but by changing the return type from <see cref="ActionResult"/> or <see cref="ActionResult{TData}"/>
		/// to <see cref="ApiResult"/> or <see cref="ApiResult{TData}"/>, ultimately allowing the response body
		/// to be an <see cref="IApiResult"/>.
		/// </summary>
		/// <example>
		/// <code>
		/// [Get("{id}"]
		/// public ApiResult Get([FromRoute] string id)
		/// {
		///		return Ok();
		/// }
		/// </code>
		/// </example>
		/// <param name="result">An <see cref="ApiResult"/> that wraps the <see cref="ActionResult"/></param>
		public static implicit operator ApiResult(ActionResult result)
		{
			return new ApiResult { ActionResult = result };
		}

		/// <summary>
		/// Renders the object as a JSON string.
		/// </summary>
		/// <returns>JSON representation of this object.</returns>
		public override string ToString()
		{
			return JsonConvert.SerializeObject(this);
		}

		/// <summary>
		/// Converts the current instance to an instance of <see cref="T:Microsoft.AspNetCore.Mvc.IActionResult" />.
		///
		/// Specifically, this function will ensure the body of the ActionResult is wrapped in an <see cref="IApiResult"/> envelope.
		/// The conversion happens automatically when returning an <see cref="ApiResult"/> from an endpoint,
		/// making the process of ensuring an envelope exists as the main body content much easier.  
		/// This implementation is based on the example provided by <see cref="ActionResult{TData}"/>.
		/// </summary>
		/// <returns>An action result with an </returns>
		public virtual IActionResult Convert()
		{
			switch (ActionResult)
			{
				case ObjectResult objectResult:
					return objectResult.Value is ApiResult
						? objectResult
						: new ObjectResult(objectResult.Value.ToApiResult())
						{
							DeclaredType = typeof(ApiResult<>).MakeGenericType(objectResult.Value.GetType()),
							StatusCode = objectResult.StatusCode
						};
				case JsonResult jsonResult:
					return jsonResult.Value is ApiResult
						? jsonResult
						: new JsonResult(jsonResult.Value.ToApiResult())
						{
							SerializerSettings = jsonResult.SerializerSettings,
							StatusCode = jsonResult.StatusCode
						};
				case ContentResult contentResult:
					return new ObjectResult(contentResult.Content.ToApiResult());
				case StatusCodeResult statusCodeResult:
					return statusCodeResult.StatusCode < (int) HttpStatusCode.BadRequest
						? new ObjectResult(new ApiResult()) { StatusCode = statusCodeResult.StatusCode, DeclaredType = typeof(ApiResult)}
						: throw new HttpStatusCodeException((HttpStatusCode) statusCodeResult.StatusCode);
				default:
					return ActionResult;
			}
		}

		#endregion
	}

	/// <summary>
	/// Result of an API request containing data.
	/// </summary>
	/// <typeparam name="TData">The type of item being returned in the response.</typeparam>
	public class ApiResult<TData> : ApiResult, IApiResult<TData>
	{
		#region Properties

		/// <summary>
		/// The data result of the response.
		/// </summary>
		public new TData Data { get; set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Implicit conversion operator that will convert be able to convert an ActionResult to an ApiResult.
		///
		/// This specifically adds the ability for a controller to be written utilizing all the baked in ActionResult functions,
		/// but by changing the return type from <see cref="ActionResult"/> or <see cref="ActionResult{TData}"/>
		/// to <see cref="ApiResult"/> or <see cref="ApiResult{TData}"/>, ultimately allowing the response body
		/// to be an <see cref="IApiResult"/>.
		/// </summary>
		/// <example>
		/// <code>
		/// [Get("{id}"]
		/// public ApiResult{TData} Get([FromRoute] string id)
		/// {
		///		return GetResult(id);
		/// }
		/// </code>
		/// </example>
		/// <param name="data">An <see cref="ApiResult"/> that wraps the <see cref="ActionResult"/></param>
		public static implicit operator ApiResult<TData>(TData data)
		{
			var apiResult = data.ToApiResult();
			apiResult.ActionResult = new ObjectResult(apiResult);

			return apiResult;
		}

		/// <summary>
		/// Implicit conversion operator that will convert be able to convert an ActionResult to an ApiResult.
		///
		/// This specifically adds the ability for a controller to be written utilizing all the baked in ActionResult functions,
		/// but by changing the return type from <see cref="ActionResult"/> or <see cref="ActionResult{TData}"/>
		/// to <see cref="ApiResult"/> or <see cref="ApiResult{TData}"/>, ultimately allowing the response body
		/// to be an <see cref="IApiResult"/>.
		/// </summary>
		/// <example>
		/// <code>
		/// [Get("{id}"]
		/// public ApiResult{TData} Get([FromRoute] string id)
		/// {
		///		return NotFound();
		/// }
		/// </code>
		/// </example>
		/// <param name="result">An <see cref="ApiResult"/> that wraps the <see cref="ActionResult"/></param>
		public static implicit operator ApiResult<TData>(ActionResult result)
		{
			return new ApiResult<TData> { ActionResult = result };
		}

		/// <summary>
		/// Implicit conversion operator that will convert be able to convert an ActionResult to an ApiResult.
		///
		/// This specifically adds the ability for a controller to be written utilizing all the baked in ActionResult functions,
		/// but by changing the return type from <see cref="ActionResult"/> or <see cref="ActionResult{TData}"/>
		/// to <see cref="ApiResult"/> or <see cref="ApiResult{TData}"/>, ultimately allowing the response body
		/// to be an <see cref="IApiResult"/>.
		/// </summary>
		/// <example>
		/// <code>
		/// [Get("{id}"]
		/// public ApiResult{TData} Get([FromRoute] string id)
		/// {
		///		return Ok(GetResult(id));
		/// }
		/// </code>
		/// </example>
		/// <param name="result">An <see cref="ApiResult"/> that wraps the <see cref="ActionResult"/></param>
		public static implicit operator ApiResult<TData>(ObjectResult result)
		{
			if (!(result.Value is TData value))
			{
				return new ApiResult<TData> { ActionResult = result };
			}

			var apiResult = value.ToApiResult();

			apiResult.ActionResult = result;

			return apiResult;
		}

		#endregion
	}

}
