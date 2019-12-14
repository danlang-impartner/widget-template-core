using System.Collections.Generic;
using System.ComponentModel;
using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Serializers;
using Mongo.Migration.Documents.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Impartner.Microservice.Common.Mongo.Models
{
	/// <summary>
	/// Represents a document that will be saved in a Mongo database, containing tenant information, and identification information.
	/// Version 0 represents the first iteration of the model and is used to interact with the MongoDB and <see cref="Repositories.MongoRepository"/>
	/// Version 1 represents an object that has implemented a migration path, allowing the document to be upgraded and downgraded.
	/// Next stage would be to add auditing properties for objects that need to declare who interacted with the data.
	/// </summary>
	public static class TenantDocument
	{
		public abstract class V0
		{
			[ReadOnly(true)]
			public ObjectId Id { get; set; }

			[JsonIgnore]
			public int TenantId { get; set; }
		}

		public abstract class V1 : V0, IDocument
		{
			#region Properties

			/// <summary>
			/// A bucket for properties that can't be serialized from BSON.
			/// </summary>
			[BsonExtraElements, JsonIgnore]
			public IDictionary<string, object> ExtraElements { get; set; }

			/// <summary>
			/// The version of the data shape. This should never be set directly, but rather use <see cref="RuntimeVersion"/> or <see cref="StartUpVersion"/>
			/// NOTE - <see cref="StartUpVersion"/> is not ready for prime time. It is not recommended to use it yet.
			/// </summary>
			[BsonElement(nameof(Version)), BsonSerializer(typeof(DocumentVersionSerializer))]
			public DocumentVersion Version { get; set; }

			#endregion
		}
	}
	}


