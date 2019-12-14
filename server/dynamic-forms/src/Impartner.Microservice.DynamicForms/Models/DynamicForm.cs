using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Impartner.Microservice.Common.Swagger.SchemaFilter;
using TenantDocument = Impartner.Microservice.Common.Mongo.Models.TenantDocument.V1;
using Mongo.Migration.Documents.Attributes;
using Mongo.Migration.Migrations;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.Annotations;

namespace Impartner.Microservice.DynamicForms.Models
{
	// NOTE - When adding new model versions, it is appropriate to make a new file containing the model and the migration(although not necessary).
	// NOTE - When creating a new file, update the static classes with a partial keyword and name the file: <modelName>V<versionNumber>.cs -> Dynamic Form v1.0.1 = DynamicFormV1_0_1.cs
	// NOTE - This actually makes the process of upgrading significantly easier especially after several version iterations.
	public static class DynamicForm
	{
		#region Migrations

		public static class Migrations
		{
			public class V1 : Migration<DynamicForm.V1>
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

		[RuntimeVersion("1.0.0")]
		[SwaggerSchemaFilter(typeof(NestedClassFilter))]
		public class V1 : TenantDocument
		{
			#region Properties

			[Required, ReadOnly(true)]
			public virtual string ObjectName { get; set; }

			[ReadOnly(true)]
			public virtual int Revision { get; set; }

			[Required]
			public virtual List<DynamicFormSection> Sections { get; set; } = new List<DynamicFormSection>();

			public virtual DynamicFormState State { get; set; }

			[ReadOnly(true)]
			public virtual int WidgetId { get; set; }

			#endregion
		}

		#endregion
	}

}
