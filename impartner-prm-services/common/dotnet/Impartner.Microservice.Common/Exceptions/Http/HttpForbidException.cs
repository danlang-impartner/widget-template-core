using System.Net;
using Impartner.Microservice.Common.Models.Responses;

namespace Impartner.Microservice.Common.Exceptions.Http
{
	/// <summary>
	/// Exception that will produce a Forbid(403) response when thrown.
	/// </summary>
	public class HttpForbidException : HttpStatusCodeException
	{
		#region Constructors

		public HttpForbidException(params ApiError[] errors)
			: base(HttpStatusCode.Forbidden, errors) { }

		#endregion
	}
}
