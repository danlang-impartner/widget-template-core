using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Impartner.Microservice.Common
{
	/// <summary>
	/// Builder type that is used to construct and lock a service into the microservice ecosystem. 
	/// </summary>
	public interface IImpartnerBuilder
	{
		#region Properties

		/// <summary>
		/// The configuration that has been loaded by the Microservice.
		/// </summary>
		IConfiguration Configuration { get; }

		/// <summary>
		/// Collection of services that have been added to the project.
		/// </summary>
		IServiceCollection ServiceCollection { get; }

		#endregion
	}
}
