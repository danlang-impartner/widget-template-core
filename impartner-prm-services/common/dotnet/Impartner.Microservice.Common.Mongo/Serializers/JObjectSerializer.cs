using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json.Linq;

namespace Impartner.Microservice.Common.Mongo.Serializers
{
	public class JObjectSerializer : SerializerBase<JObject>
	{
		#region Public Methods

		public override JObject Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
		{
			var bsonDoc = BsonDocumentSerializer.Instance.Deserialize(context);
			return JObject.Parse(bsonDoc.ToString());
		}

		public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, JObject value)
		{
			var bsonDoc = BsonDocument.Parse(value.ToString());
			BsonDocumentSerializer.Instance.Serialize(context, bsonDoc);
		}

		#endregion
	}
}