using System.Net;
using Impartner.Microservice.Common.Models.Responses;

namespace Impartner.Microservice.Common.Exceptions.Http
{
	/// <summary>
	/// Exception that will produce a Conflict(409) response when thrown.
	/// </summary>
	public class HttpConflictException : HttpStatusCodeException
	{
		#region Constructors

		public HttpConflictException(params ApiError[] errors)
			: base(HttpStatusCode.Conflict, errors) { }

		#endregion
	}
}
