using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Impartner.Microservice.Common.Mongo.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Impartner.Microservice.Common.Mongo.HealthChecks
{
	/// <summary>
	/// Health check that will validate that the connection between a service and the Mongo database declared in the config are healthy.
	/// </summary>
	public class MongoDbHealthCheck : IHealthCheck
	{
		#region Fields

		/// <summary>
		/// Cache of all the mongo clients that have been ran so far; These differ by the Mongo URL/authentication used to connect to the database.
		/// </summary>
		private static readonly ConcurrentDictionary<MongoClientSettings, MongoClient> MongoClientCache = new ConcurrentDictionary<MongoClientSettings, MongoClient>();

		/// <summary>
		/// The settings that will be used by the health check.
		/// </summary>
		private readonly MongoClientSettings _mongoClientSettings;

		/// <summary>
		/// The settings loaded from configuration concerning the Mongo database.
		/// </summary>
		private readonly MongoDbSettings _mongoDbSettings;

		/// <summary>
		/// Logging service.
		/// </summary>
		private readonly ILogger<MongoDbHealthCheck> _logger;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the MongoDb health check.
		/// </summary>
		/// <param name="mongoDbSettings">Mongo settings from the configuration file.</param>
		/// <param name="logger"></param>
		public MongoDbHealthCheck(IOptions<MongoDbSettings> mongoDbSettings, ILogger<MongoDbHealthCheck> logger)
		{
			_mongoDbSettings = mongoDbSettings?.Value ?? throw new ArgumentNullException(nameof(mongoDbSettings));

			// If there is a malformed connection string, this will fail immediately.
			_mongoClientSettings = MongoClientSettings.FromUrl(new MongoUrl(_mongoDbSettings.ConnectionString));

			// Decrease the timeout times; By default these are all 30 seconds; Since we want to check for healthiness, if responses take more then 10 seconds, then it is not healthy.
			_mongoClientSettings.ServerSelectionTimeout = _mongoClientSettings.ConnectTimeout = _mongoClientSettings.HeartbeatTimeout = TimeSpan.FromSeconds(10);
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Runs a health check against all databases related to this service that are found at the given connection string.
		/// </summary>
		/// <param name="context">A context object associated with the current execution.</param>
		/// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> that can be used to cancel the health check.</param>
		/// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that completes when the health check has finished, yielding the status of the component being checked.</returns>
		public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
		{
			_logger.LogDebug("Starting health check");
			var stopwatch = Stopwatch.StartNew();

			var mongoClient = MongoClientCache.GetOrAdd(_mongoClientSettings, settings => new MongoClient(settings));

			var databaseNames = (await mongoClient.ListDatabaseNames().ToListAsync(cancellationToken)).Where(value => value.StartsWith(_mongoDbSettings.DatabaseName));
			var healthAdditionalData = new Dictionary<string, object> {{_mongoDbSettings.DatabaseName, databaseNames}};

			var databaseTasks = databaseNames.Select(databaseName => Task.Run(async () =>
				{
					try
					{
						var database = mongoClient.GetDatabase(databaseName);

						// Check that the connection works.
						var ping = await database.RunCommandAsync<BsonDocument>(new BsonDocument {{"ping", 1}}, default,
							cancellationToken);

						if (!ping.Contains("ok") || ping["ok"].IsDouble && (int)ping["ok"].AsDouble != 1 || ping["ok"].IsInt32 && ping["ok"].AsInt32 != 1)
						{
							throw new Exception($"Could not communicate with the MongoDB: {databaseName}");
						}

						// Check that reading from the database works.
						await database.ListCollections(
							new ListCollectionsOptions{ Filter = Builders<BsonDocument>.Filter.Eq("name", nameof(MongoDbHealthCheck))}
						).ToListAsync(cancellationToken);

						return (isSuccessful: true, databaseName, null);
					}
					catch (Exception exception)
					{
						_logger.LogCritical(exception, $"There was a problem while testing the health of the Mongo DB: {databaseName}");
						return (isSuccessful: false, databaseName, exception);
					}
				}, cancellationToken)
			);

			var results = await Task.WhenAll(databaseTasks);

			stopwatch.Stop();

			_logger.LogInformation(
				"MongoDB Health Check for {@MongoUrl} and Database {Database} took {Elapsed:00000} ms",
				_mongoClientSettings.Server,
				_mongoDbSettings.DatabaseName,
				stopwatch.ElapsedMilliseconds
			);

			if (results.All(value => value.isSuccessful))
			{
				return HealthCheckResult.Healthy
				(
					"All connected Mongo Databases have been tested for read, write, and delete and are healthy and available.",
					healthAdditionalData
				);
			}

			var unhealthyResults = results.Where(value => !value.isSuccessful).ToList();

			return HealthCheckResult.Unhealthy(
				$"The following Mongo Databases could not fulfill requirements for health checks: {string.Join(", ", unhealthyResults.Select(value => value.databaseName))}",
				new AggregateException(unhealthyResults.Select(value => value.exception)),
				healthAdditionalData
			);
		}

		#endregion
	}
}
