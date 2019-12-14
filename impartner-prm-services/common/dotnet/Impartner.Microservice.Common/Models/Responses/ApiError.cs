using System;
using System.Collections;
using Impartner.Common.Api;
using Impartner.Microservice.Common.Swagger.SchemaFilter;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace Impartner.Microservice.Common.Models.Responses
{
	/// <summary>
	/// Severity of an API Error.
	/// </summary>
	public enum ApiErrorSeverity
	{
		Error,
		Warning,
		Information,
		Debug,
		Critical
	}

	/// <summary>
	/// Error produced from an API.
	/// </summary>
	[SwaggerSchemaFilter(typeof(ApiErrorSchemaFilter))]
	public class ApiError : IApiError
	{
		#region Properties

		/// <summary>
		/// Collection of additional data that can be used by the client to properly portray the error.
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public IDictionary AdditionalData { get; set; }

		/// <summary>
		/// The error code; This should be used for localization of the error message.
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		/// Details of the error. This should be localized to the current API, but may be localized differently on the client.
		///
		/// Dynamic values should be inserted into <see cref="AdditionalData"/> to allow for localization.
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// The severity of the error.
		/// </summary>
		public ApiErrorSeverity Severity { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs an API Error with a code and message.
		/// </summary>
		/// <param name="code">An error code; This will be used for localization.</param>
		/// <param name="message">The default error message. This </param>
		/// <param name="severity"></param>
		/// <param name="additionalData"></param>
		public ApiError(string code, string message, ApiErrorSeverity severity = ApiErrorSeverity.Error, IDictionary additionalData = null)
		{
			Code = code;
			Message = message;
			AdditionalData = additionalData;
			Severity = severity;
		}

		public ApiError(string code, Exception exception, ApiErrorSeverity severity = ApiErrorSeverity.Critical)
		{
			AdditionalData = exception.Data;
			Message = exception.Message;
			Code = code;
			Severity = severity;
		}

		#endregion
	}
}
