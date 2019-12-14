using System;
using System.Collections.Generic;
using Impartner.Microservice.Common.Models;
using Impartner.Microservice.Common.Swagger.SchemaFilter;
using Mongo.Migration.Documents.Attributes;
using Mongo.Migration.Migrations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using Swashbuckle.AspNetCore.Annotations;
using TenantDocument = Impartner.Microservice.Common.Mongo.Models.TenantDocument.V1;

namespace Impartner.Microservice.WidgetRegistration.Models
{
	public static class WidgetEntry
	{
		#region Migrations

		public static class Migrations
		{
			public class V1 : Migration<WidgetEntry.V1>
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

		[RuntimeVersion("1.0.0")]
		[SwaggerSchemaFilter(typeof(NestedClassFilter))]
		public class V1 : TenantDocument
		{
			public User CreatedBy { get; set; }
			public User UpdatedBy  { get; set; }
			public DateTime CreatedAt { get; set; }
			public DateTime UpdateAt { get; set; }
			public string Name { get; set; }
			public string Type { get; set; }
			public bool IsActive { get; set; }
			public string LatestVersion { get; set; }

			[BsonDictionaryOptions(Representation = DictionaryRepresentation.ArrayOfDocuments)]
			public Dictionary<string, WidgetVersion> Versions { get; set; } = new Dictionary<string, WidgetVersion>();

			public void AddVersion(ManifestWidgetVersion version, User user)
			{
				var now = DateTime.UtcNow;
				UpdatedBy = user;
				UpdateAt = now;
				var versionNumber = version.Manifest.Version;
				var widgetVersion = version.WidgetVersion;
				widgetVersion.CreatedBy = user;
				widgetVersion.CreatedAt = now;
				Versions.Add(versionNumber, widgetVersion);
				LatestVersion = versionNumber;
			}
		}
	}
}
