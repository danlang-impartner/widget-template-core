using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Impartner.Microservice.Common.Authentication
{
	/// <summary>
	/// Event handlers for Authentication events.
	/// </summary>
	public static class AuthenticationEvents
	{
		#region Public Methods

		/// <summary>
		/// Handler for a failed authentication event.
		/// </summary>
		public static Task AuthenticationFailed(AuthenticationFailedContext _)
		{
			// TODO - Add functionality here.
			return Task.CompletedTask;
		}

		/// <summary>
		/// Handler for a success authentication event.
		/// </summary>
		public static Task TokenValidated(TokenValidatedContext _)
		{
			// TODO - Add functionality here.
			return Task.CompletedTask;
		}

		#endregion
	}
}
