using Impartner.Microservice.Common.Mongo.HealthChecks;
using Microsoft.Extensions.DependencyInjection;

namespace Impartner.Microservice.Common.Mongo.Extensions
{
	/// <summary>
	/// Extensions methods for the <see cref="IHealthChecksBuilder"/>.
	/// </summary>
	public static class HealthChecksBuilderExtensions
	{
		#region Public Methods

		/// <summary>
		/// Adds health checks around the Mongo database, and naming them appropriately so they can be reported properly.
		/// </summary>
		/// <param name="builder">The health check builder.</param>
		public static IHealthChecksBuilder AddMongoDbHealthChecks(this IHealthChecksBuilder builder)
		{
			builder.AddTypeActivatedCheck<MongoDbHealthCheck>(nameof(MongoDbHealthCheck));

			return builder;
		}

		#endregion
	}
}
