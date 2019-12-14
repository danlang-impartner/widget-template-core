using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Impartner.Microservice.Localization.Swagger.SchemaFilter;
using Mongo.Migration.Migrations;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.Annotations;
using TenantDocument = Impartner.Microservice.Common.Mongo.Models.TenantDocument.V1;

namespace Impartner.Microservice.Localization.Models
{
	// NOTE - When adding new model versions, it is appropriate to make a new file containing the model and the migration(although not necessary).
	// NOTE - When creating a new file, update the static classes with a partial keyword and name the file: <modelName>V<versionNumber>.cs -> Dynamic Form v1.0.1 = DynamicFormV1_0_1.cs
	// NOTE - This actually makes the process of upgrading significantly easier especially after several version iterations.
	public static class LocalizationModel
	{
		#region Migrations

		public static class Migrations
		{

			public class V1 : Migration<LocalizationModel.V1>
			{
				#region Constructors

				public V1() : base("1.0.0") {}

				#endregion

				#region Public Methods

				public override void Down(BsonDocument document) {}
				public override void Up(BsonDocument document) {}

				#endregion
			}
		}

		#endregion

		#region Models

		[SwaggerSchemaFilter(typeof(LocalizationModelSchemaFilter))]
		public class V1 : TenantDocument
		{
			#region Properties

			[Required]
			public Dictionary<string, Dictionary<string, string>> Languages { get; set; }

			[Required, ReadOnly(true)]
			public string ObjectId { get; set; }

			[Required, ReadOnly(true)]
			public string ObjectName { get; set; }

			#endregion
		}

		#endregion
	}
}
