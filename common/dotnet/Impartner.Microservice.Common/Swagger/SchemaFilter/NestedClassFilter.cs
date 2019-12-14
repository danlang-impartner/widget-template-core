using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Impartner.Microservice.Common.Swagger.SchemaFilter
{
	/// <summary>
	/// Schema filter that will adjust the title of an object to include the owner class e.g. OwnerClass.NestedClass
	/// </summary>
	public class NestedClassFilter : ISchemaFilter
	{
		#region Public Methods

		public virtual void Apply(Schema schema, SchemaFilterContext context)
		{
			schema.Title = string.Join('.', context.SystemType.ReflectedType?.Name, context.SystemType.Name).Trim('.');
		}

		#endregion
	}
}
