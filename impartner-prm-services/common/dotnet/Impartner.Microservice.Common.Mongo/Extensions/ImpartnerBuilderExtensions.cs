using System;
using System.Linq;
using Impartner.Microservice.Common.Mongo.Models;
using Impartner.Microservice.Common.Mongo.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Mongo.Migration.Extensions;
using Mongo.Migration.Startup.DotNetCore;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace Impartner.Microservice.Common.Mongo.Extensions
{
	/// <summary>
	/// Extensions for adding features into a Impartner microservice regarding mongodb.
	/// </summary>
	public static class ImpartnerBuilderExtensions
	{
		#region Public Methods

		/// <summary>
		/// Connect the project to a mongo database.
		/// </summary>
		/// <param name="builder">Builder for creating an Impartner service.</param>
		/// <param name="configureMongo">Additional configuration for Mongo.</param>
		public static IImpartnerBuilder AddMongo(this IImpartnerBuilder builder, Action<IMongoBuilder> configureMongo = null)
		{
			var mongoBuilder = new MongoBuilder();
			var camelCaseBsonConvention = new ConventionPack {new CamelCaseElementNameConvention()};

			builder.ServiceCollection.Configure<MongoDbSettings>
			(
				builder.Configuration.GetSection(nameof(MongoDbSettings))
			);
			builder.ServiceCollection.Configure<MongoMigrationSettings>
			(
				builder.Configuration.GetSection(nameof(MongoDbSettings))
			);

			builder.ServiceCollection.AddScoped<IMongoRepository, MongoRepository>
			(
				provider =>
				{
					var (httpProvider, mongoSettings) = (
						provider.GetRequiredService<IHttpContextAccessor>(),
						provider.GetRequiredService<IOptions<MongoDbSettings>>().Value
					);
					var client = new MongoClient(mongoSettings.ConnectionString);

					return new MongoRepository(client, httpProvider, mongoSettings);
				}
			);

			if (builder.ServiceCollection.All(value => value.ServiceType != typeof(IHttpContextAccessor)))
			{
				builder.ServiceCollection.AddHttpContextAccessor();
			}
			
			mongoBuilder.AddConvention(nameof(camelCaseBsonConvention), camelCaseBsonConvention);

			builder.ServiceCollection.AddMigration();
			
			configureMongo?.Invoke(mongoBuilder);

			return builder;
		}

		#endregion
	}
}
