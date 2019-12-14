using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Impartner.Microservice.Common.Extensions
{
	/// <summary>
	/// Extension functions for <see cref="HttpContext"/>.
	/// </summary>
	public static class HttpContextExtensions
	{
		#region Public Methods

		/// <summary>
		/// Gets the value of a claim type if it can be found on the identity.
		/// </summary>
		/// <param name="httpContext">The context of the current HTTP request.</param>
		/// <param name="claimType">The type of claim to retrieve from the user identity.</param>
		/// <returns>The value of the claim associated with the user identity as a string.</returns>
		public static string GetClaimValue(this HttpContext httpContext, string claimType) =>
			httpContext?.User?.FindFirst(claimType)?.Value ?? throw new KeyNotFoundException($"Claim type {claimType} not found in claim collection");

		/// <summary>
		/// Gets the value of a claim type if it can be found on the identity.
		/// </summary>
		/// <typeparam name="T">The type of the result to return.</typeparam>
		/// <param name="httpContext">The context of the current HTTP request.</param>
		/// <param name="claimType">The type of claim to retrieve from the user identity.</param>
		/// <returns>The value of the claim associated with the user identity converted to the provided type T.</returns>
		public static T GetClaimValue<T>(this HttpContext httpContext, string claimType)
			=> (T) Convert.ChangeType(GetClaimValue(httpContext, claimType), typeof(T));

		/// <summary>
		/// Try to retrieve the claim value if it exists and is 
		/// </summary>
		/// <typeparam name="T">The type of the result to return.</typeparam>
		/// <param name="httpContext">The context of the current HTTP request.</param>
		/// <param name="claimType">The type of claim to retrieve.</param>
		/// <param name="value">The value of the claim associated with the user identity converted to the provided type T.</param>
		/// <returns>True if the cast can be accomplished, false otherwise.</returns>
		public static bool TryGetClaimValue<T>(this HttpContext httpContext, string claimType, out T value)
		{
			try
			{
				value = GetClaimValue<T>(httpContext, claimType);
				return true;
			}
			catch
			{
				value = default;
				return false;
			}
		}

		/// <summary>
		/// Adds Pagination to the response by adding a Links header, with links to pages the client would be interested in viewing.
		/// </summary>
		/// <param name="httpContext">The context of the current HTTP request.</param>
		/// <param name="skip">Number of elements to skip.</param>
		/// <param name="take">Number of elements to take.</param>
		/// <param name="totalCount">Total number of elements available.</param>
		public static void AddPaginationHeaders(this HttpContext httpContext, int skip, int take, int totalCount)
		{
			// TODO - Move to an infrastructure function.
			var pageCount = Math.Max(0, (totalCount - 1) / take);
			var currentPage = Math.Min(Math.Max(0, (skip - 1) / take), pageCount);
			var lastPageTake = totalCount % take == 0 ? take : totalCount % take;
			var requestUrl = httpContext.GetRequestUrl();

			var linkHeaderValues = new List<string>();

			// Only add next if there is another page to traverse.
			if (currentPage < pageCount)
			{
				linkHeaderValues.Add($"<{requestUrl}?skip={(currentPage + 1) * take}&take={take}>; rel=\"next\"");
			}

			// Only add prev and first, if not already on the first page.
			if (currentPage > 0)
			{
				linkHeaderValues.Add($"<{requestUrl}?skip={(currentPage - 1) * take}&take={take}>; rel=\"prev\"");
				linkHeaderValues.Add($"<{requestUrl}?skip=0&take={take}>; rel=\"first\"");
			}

			// Always add last to help anchor the client to the available range.
			linkHeaderValues.Add($"<{requestUrl}?skip={pageCount * take}&take={lastPageTake}>; rel=\"last\"");

			httpContext.Response.Headers["Links"] = new StringValues(linkHeaderValues.ToArray());
		}

		/// <summary>
		/// Gets the request URL. If the url was forwarded from a proxy, attempt to rebuild the URL that the request was accessed from.
		///
		/// TODO - Once this moves into a new environment, this should be tested, as it's hard to say whether these same rules would work in the Kubernetes environment.
		/// </summary>
		/// <param name="httpContext">The context of the current HTTP request.</param>
		/// <param name="includeQuery">Flag to include or exclude the query.</param>
		/// <returns>The URL used to make the request.</returns>
		public static string GetRequestUrl(this HttpContext httpContext, bool includeQuery = false)
		{
			var queryString = includeQuery ? httpContext.Request.QueryString : QueryString.Empty;

			var requestPath = httpContext.Request.Path.HasValue ? httpContext.Request.Path.Value : "";
			var forwardedPrefix = httpContext.Request.Headers["X-Forwarded-Prefix"].FirstOrDefault();

			if (string.IsNullOrEmpty(forwardedPrefix) && requestPath.Contains("/ms/"))
			{
				requestPath = requestPath.Replace("/ms/", $"{forwardedPrefix}/");
			}

			return $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{requestPath}{queryString}";
		}

		#endregion
	}
}
