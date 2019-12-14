using System;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace Impartner.Microservice.Common.Mongo
{
	public interface IMongoBuilder
	{
		#region Public Methods

		IMongoBuilder AddClassMap<T>(Action<BsonClassMap<T>> configureClassMap);

		IMongoBuilder AddConvention(string name, IConventionPack convention, Func<Type, bool> filter = null);
		IMongoBuilder AddSerializer<T>(IBsonSerializer<T> serializer);

		#endregion
	}
}
