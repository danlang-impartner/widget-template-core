using System.Collections.Generic;
using Impartner.Microservice.Common.Swagger.SchemaFilter;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using LocalizationModel = Impartner.Microservice.Localization.Models.LocalizationModel.V1;

namespace Impartner.Microservice.Localization.Swagger.SchemaFilter
{
	/// <summary>
	/// Adds an example to the <see cref="LocalizationModel"/> schema for Swagger.
	/// </summary>
	public class LocalizationModelSchemaFilter : NestedClassFilter
	{
		#region Public Methods

		/// <summary>
		/// Applies the schema change. This includes an example of localization model, but also includes adjusting the class title to include a nested class model.
		/// </summary>
		public override void Apply(Schema schema, SchemaFilterContext context)
		{
			base.Apply(schema, context);

			schema.Example = new LocalizationModel
			{
				Languages = new Dictionary<string, Dictionary<string, string>>
				{
					{"en-us", new Dictionary<string, string> {{"TranslationKey1", "..."}, {"TranslationKey2", "..."}}},
					{"de-de", new Dictionary<string, string> {{"TranslationKey1", "..."}, {"TranslationKey2", "..."}}}
				}
			};
		}

		#endregion
	}
}
