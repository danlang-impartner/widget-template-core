using System.Collections.Generic;
using Impartner.Microservice.Common.Models.Responses;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Impartner.Microservice.Common.Swagger.SchemaFilter
{
	/// <summary>
	/// Swagger Schema filter that cleans up the <see cref="ApiError"/> example.
	/// </summary>
	public class ApiErrorSchemaFilter : ISchemaFilter
	{
		#region Public Methods

		/// <summary>
		/// Updates the schema example for <see cref="ApiError"/> with an example of using Additional data for a message with dynamic data.
		/// </summary>
		public void Apply(Schema schema, SchemaFilterContext context)
		{
			schema.Example = new ApiError
			(
				"ExampleErrorCode",
				"Example message with data that will be retrieved from additional data map: $data",
				additionalData: new Dictionary<string, object>
				{
					{ "data", new { ExampleProperty1 = "...", ExampleProperty2 = "..." } }
				}
			);
		}

		#endregion
	}
}
