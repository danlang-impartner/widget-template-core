using System.Net;
using Impartner.Microservice.Common.Models.Responses;

namespace Impartner.Microservice.Common.Exceptions.Http
{
	/// <summary>
	/// Exception that will produce a Not Found(401) response when thrown.
	/// </summary>
	public class HttpUnauthorizedException : HttpStatusCodeException
	{
		#region Constructors

		public HttpUnauthorizedException(params ApiError[] errors)
			: base(HttpStatusCode.Unauthorized, errors) { }

		#endregion
	}
}
