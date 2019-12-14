using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Impartner.Microservice.Common
{
	internal sealed class ImpartnerBuilder : IImpartnerBuilder
	{
		#region Properties

		/// <summary>
		/// The configuration that has been loaded by this service.
		/// </summary>
		public IConfiguration Configuration { get; }

		/// <summary>
		/// Collection of services that contain Dependency Injection elements that have been added to
		/// this service.
		/// </summary>
		public IServiceCollection ServiceCollection { get; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs an Impartner Ecosystem builder.
		/// </summary>
		/// <param name="configuration">The configuration that has been loaded by this service.</param>
		/// <param name="serviceCollection">
		/// Collection of services that contain Dependency Injection elements that have been added to
		/// this service.
		/// </param>
		internal ImpartnerBuilder(IConfiguration configuration, IServiceCollection serviceCollection)
		{
			Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			ServiceCollection = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));
		}

		#endregion
	}
}
