using System;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace Impartner.Microservice.Common.Mongo
{
	internal sealed class MongoBuilder : IMongoBuilder
	{
		#region Public Methods

		public IMongoBuilder AddClassMap<T>(Action<BsonClassMap<T>> configureClassMap)
		{
			BsonClassMap.RegisterClassMap(configureClassMap);
			return this;
		}

		public IMongoBuilder AddConvention(string name, IConventionPack convention, Func<Type, bool> filter = null)
		{
			ConventionRegistry.Register(name, convention, filter ?? ApplyToAll);
			return this;
		}

		public IMongoBuilder AddSerializer<T>(IBsonSerializer<T> serializer)
		{
			BsonSerializer.RegisterSerializer(serializer);
			return this;
		}

		#endregion

		#region Private Methods

		private static bool ApplyToAll(Type type) => true;

		#endregion
	}
}
