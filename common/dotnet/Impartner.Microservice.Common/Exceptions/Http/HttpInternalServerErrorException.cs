using System.Net;
using Impartner.Microservice.Common.Models.Responses;

namespace Impartner.Microservice.Common.Exceptions.Http
{
	/// <summary>
	/// Exception that will produce a Internal Server Error(500) response when thrown.
	/// </summary>
	public class HttpInternalServerErrorException : HttpStatusCodeException
	{
		#region Constructors

		public HttpInternalServerErrorException(params ApiError[] errors)
			: base(HttpStatusCode.InternalServerError, errors) { }

		#endregion
	}
}
