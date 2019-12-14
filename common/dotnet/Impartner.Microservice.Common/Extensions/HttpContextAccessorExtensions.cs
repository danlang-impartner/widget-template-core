using Impartner.Common.Security.Constants;
using Impartner.Microservice.Common.Utilities;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace Impartner.Microservice.Common.Extensions
{
	public static class HttpContextAccessorExtensions
	{
		private static readonly Regex TruncateEnvironmentContextRegex = StringUtilities.CreateTruncateRegex(max: 48);

		public static string GetEnvironmentContext(this IHttpContextAccessor contextAccessor)
		{
			return StringUtilities.ApplyTruncateRegex
			(
				$"{contextAccessor.HttpContext.Request?.Headers?["X-Environment-Context"]}",
				TruncateEnvironmentContextRegex
			);
		}

		public static int GetTenantId(this IHttpContextAccessor contextAccessor)
		{
			return contextAccessor.HttpContext.GetClaimValue<int>(ClaimTypes.TenantId);
		}
	}
}
