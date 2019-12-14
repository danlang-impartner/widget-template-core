using System.Net;
using Impartner.Microservice.Common.Models.Responses;

namespace Impartner.Microservice.Common.Exceptions.Http
{
	/// <summary>
	/// Exception that will produce a Bad Request(400) response when thrown.
	/// </summary>
	public class HttpBadRequestException : HttpStatusCodeException
	{
		#region Constructors

		public HttpBadRequestException(params ApiError[] errors)
			: base(HttpStatusCode.BadRequest, errors) { }

		#endregion
	}
}
