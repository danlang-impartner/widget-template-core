using Impartner.Microservice.Common.Mongo.Serializers;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Impartner.Microservice.DynamicForms.Models
{
	public class Condition
	{
		public string Fact { get; set; }
		[JsonConverter(typeof(StringEnumConverter))]
		public Operator Operator { get; set; }

		[BsonSerializer(typeof(SafeJsonContentSerializer))]
		public object Value { get; set; }
	}
}
