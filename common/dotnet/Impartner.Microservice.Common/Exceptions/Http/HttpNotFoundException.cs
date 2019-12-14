using System.Net;
using Impartner.Microservice.Common.Models.Responses;

namespace Impartner.Microservice.Common.Exceptions.Http
{
	/// <summary>
	/// Exception that will produce a Not Found(404) response when thrown.
	/// </summary>
	public class HttpNotFoundException : HttpStatusCodeException
	{
		#region Constructors

		public HttpNotFoundException(params ApiError[] errors)
			: base(HttpStatusCode.NotFound, errors) {}

		#endregion
	}
}
