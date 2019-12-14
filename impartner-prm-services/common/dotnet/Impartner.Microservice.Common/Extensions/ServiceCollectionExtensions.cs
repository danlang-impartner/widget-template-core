using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Impartner.Microservice.Common.Extensions
{
	/// <summary>
	/// Collection of functions that extend the customization of the service and the dependency injector.
	/// </summary>
	public static class ServiceCollectionExtensions
	{
		#region Public Methods

		/// <summary>
		/// Adds Impartner Ecosystem builder to the service.
		/// </summary>
		/// <param name="serviceCollection">The Dependency Injection service collection of the service.</param>
		/// <param name="configuration">Currently loaded configuration of the service.</param>
		/// <param name="configureHealthChecks">Configuration for the health checks used by this service.</param>
		/// <returns>
		/// An Impartner Ecosystem builder that can be used to configure the service and how it
		/// interacts with the ecosystem.
		/// </returns>
		public static IImpartnerBuilder AddImpartner(
			this IServiceCollection serviceCollection,
			IConfiguration configuration,
			Action<IHealthChecksBuilder> configureHealthChecks = null
		)
		{
			var healthChecksBuilder = serviceCollection.AddHealthChecks();

			configureHealthChecks?.Invoke(healthChecksBuilder);

			return new ImpartnerBuilder(configuration, serviceCollection);
		}

		#endregion
	}
}
