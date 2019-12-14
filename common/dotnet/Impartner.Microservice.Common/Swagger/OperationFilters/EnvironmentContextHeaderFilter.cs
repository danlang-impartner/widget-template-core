using System.Collections.Generic;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Impartner.Microservice.Common.Swagger.OperationFilters
{
	/// <summary>
	/// Filter will add an X-Environment-Context value that will add into the header when the  message is sent.
	/// </summary>
	public class EnvironmentContextHeaderFilter : IOperationFilter
	{
		/// <summary>
		/// Adds an X-Environment-Context parameter to each operation.
		/// </summary>
		/// <param name="operation">The operation to add to.</param>
		/// <param name="context">The current context.</param>
		public void Apply(Operation operation, OperationFilterContext context)
		{
			operation.Parameters = operation.Parameters ?? new List<IParameter>();
			operation.Parameters.Add(
				new NonBodyParameter
				{
					Name = "X-Environment-Context",
					In = "header",
					Type = "string",
					Required = true,
					Default = "local"
				}
			);
		}
	}
}
