using System;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;

namespace Impartner.Microservice.Common.Mongo.Serializers
{
	/// <summary>
	/// Serializes the object as actual JSON so that the appropriate content is actually deserialized
	/// without being strict about typing.
	/// </summary>
	public class SafeJsonContentSerializer : IBsonSerializer<object>
	{
		#region Properties

		/// <summary>Gets the type of the value.</summary>
		/// <value>The type of the value.</value>
		public Type ValueType => typeof(object);

		#endregion

		#region Public Methods

		/// <summary>Deserializes a value from BSON to JSON to <see cref="object"/>.</summary>
		/// <param name="context">The deserialization context including the reader and current.</param>
		/// <param name="args">The deserialization args.</param>
		/// <returns>Object representing the result of the deserialization.</returns>
		public object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
		{
			// Value is expected to be stored as a JSON string, but backwards compatibility will require this to be handled more delicately.
			// If the value is not a JSON string, return the result of the BSON deserialization.
			var result = BsonSerializer.Deserialize<object>(context.Reader);

			// If the result is not a string then the value is probably
			if (!(result is string stringValue))
			{
				return result;
			}

			try
			{
				return JsonConvert.DeserializeObject(stringValue);
			}
			catch
			{
				// If the string can't be deserialized from JSON,
				// it is probably just a normal string.
				return result;
			}
		}

		/// <summary>Serializes a value.</summary>
		/// <param name="context">The serialization context.</param>
		/// <param name="args">The serialization args.</param>
		/// <param name="value">The value.</param>
		public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
		{
			BsonSerializer.Serialize(context.Writer, JsonConvert.SerializeObject(value), args: args);
		}

		#endregion
	}
}
