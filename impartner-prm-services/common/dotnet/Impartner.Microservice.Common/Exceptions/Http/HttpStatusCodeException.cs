using System.Net;
using Impartner.Microservice.Common.Models.Responses;

namespace Impartner.Microservice.Common.Exceptions.Http
{
	/// <summary>
	/// Exception that will produce a response with the provided status code when thrown.
	/// </summary>
	public class HttpStatusCodeException : ImpartnerException
	{
		#region Properties

		/// <summary>
		/// Simple message that represents the <see cref="HttpStatusCodeException"/>.
		/// </summary>
		public override string Message => $"HTTP Request resulted in a response with status code: {StatusCode}. See {nameof(Errors)} for more details.";
		#endregion

		#region Constructors

		/// <summary>
		/// Constructs an exception that will produce a response with the provided status code when thrown.
		/// </summary>
		/// <param name="statusCode">The status code for the response.</param>
		/// <param name="errors">Collection of errors associated with this exception.</param>
		public HttpStatusCodeException(HttpStatusCode statusCode, params ApiError[] errors)
			: base(errors)
		{
			StatusCode = statusCode;
		}

		#endregion
	}
}
