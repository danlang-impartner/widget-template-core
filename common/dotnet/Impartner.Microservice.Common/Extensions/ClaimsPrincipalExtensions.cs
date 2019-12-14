using Impartner.Microservice.Common.Models;
using System;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using ImpartnerClaimTypes = Impartner.Common.Security.Constants.ClaimTypes;

namespace Impartner.Microservice.Common.Extensions
{
	public static class ClaimsPrincipalExtensions
	{
		/// <summary>Get User claims info from ClaimsPrincipal.</summary>
		/// <param name="claimsPrincipal">The ClaimsPrincipal with the data to extract.</param>
		/// <returns>Instance of a <see cref="Impartner.Microservice.Common.Models.User"/> model.</returns>
		public static User ToUserInfo(this ClaimsPrincipal claimsPrincipal)
		{
			var userId = claimsPrincipal.ToUserId();
			var username = claimsPrincipal?.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var firstName = claimsPrincipal?.Claims.SingleOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
			var lastName = claimsPrincipal?.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;

			if (String.IsNullOrWhiteSpace(userId) || String.IsNullOrWhiteSpace(username) ||
				String.IsNullOrWhiteSpace(firstName) || String.IsNullOrWhiteSpace(lastName))
			{
				throw new AuthenticationException($"One or more required claim values are missing\nUserId: {userId}\nUserName: {username}\nFirstName: {firstName}\nLastName: {lastName}");
			}

			return new User
			{
				UserId = userId,
				Username = username,
				FirstName = firstName,
				LastName = lastName
			};
		}

		public static string ToUserId(this ClaimsPrincipal claimsPrincipal)
		{
			return claimsPrincipal?.Claims.SingleOrDefault(c => c.Type == ImpartnerClaimTypes.UserId)?.Value;
		}

		public static string ToTenantId(this ClaimsPrincipal claimsPrincipal)
		{
			return claimsPrincipal.Claims.SingleOrDefault(c => c.Type == ImpartnerClaimTypes.TenantId)?.Value;
		}
	}
}
