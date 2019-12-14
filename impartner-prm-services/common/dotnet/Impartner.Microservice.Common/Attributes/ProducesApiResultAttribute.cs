using System;
using System.Net;
using Impartner.Microservice.Common.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace Impartner.Microservice.Common.Attributes
{
	/// <summary>
	/// Attribute that will enrich an endpoint with details on the response it will generate.
	/// </summary>
	public class ProducesApiResultAttribute : SwaggerResponseAttribute
	{
		#region Properties

		/// <summary>
		/// Type of <see cref="ApiResult"/>.
		/// </summary>
		private static Type ApiResultType => typeof(ApiResult<>);

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes an instance of <see cref="T:Microsoft.AspNetCore.Mvc.ProducesResponseTypeAttribute" />.
		/// </summary>
		/// <param name="type">The <see cref="P:Microsoft.AspNetCore.Mvc.ProducesResponseTypeAttribute.Type" /> of object that is going to be written in the response.</param>
		/// <param name="statusCode">The HTTP response status code.</param>
		/// <param name="description">Description of the details for this status code.</param>
		public ProducesApiResultAttribute(HttpStatusCode statusCode, Type type = null, string description = null)
			: base((int) statusCode, description, type != null ? ApiResultType.MakeGenericType(type) : ApiResultType ) { }

		#endregion
	}
}
