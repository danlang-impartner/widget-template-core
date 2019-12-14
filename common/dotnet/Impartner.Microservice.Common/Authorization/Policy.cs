using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Impartner.Common.Security;
using Impartner.Common.Security.Constants;
using Microsoft.AspNetCore.Authorization;

namespace Impartner.Microservice.Common.Authorization
{
	/// <summary>
	/// Collection of policies names and access to policy builders for the policy.
	/// </summary>
	public class Policy
	{
		#region Fields

		/// <summary>
		/// Collection of available policies that are available to be used by an Impartner service.
		/// </summary>
		private static readonly IDictionary<PolicyNames, Action<AuthorizationPolicyBuilder>> AvailablePolicies = 
			// Readonly dictionary used to prevent changing the available policies at runtime.            
			new ReadOnlyDictionary<PolicyNames, Action<AuthorizationPolicyBuilder>>(
				new Dictionary<PolicyNames, Action<AuthorizationPolicyBuilder>>
				{
					// Policy to validate that the user's tenant id is available.
					{ 
						PolicyNames.TenantId, 
						builder => CheckClaim
						(
							builder, 
							ClaimTypes.TenantId, 
							value => int.TryParse(value, out var tenantId) && tenantId > 0
						)
					},

					// Policy to validate that the user id is available.
					{ PolicyNames.UserId, builder => CheckClaim(builder, ClaimTypes.UserId) },
                    
					// Policy to validate that the user's account id is available.
					{ PolicyNames.AccountId, builder => CheckClaim(builder, ClaimTypes.AccountId) },
                    
					// Policy to validate that the user is an admin.
					{
						PolicyNames.IsAdmin, 
						builder => CheckClaim(
							builder, 
							ClaimTypes.UserType,
							value => value.Equals(nameof(AuthorizationRuleTarget.Admin))
						)
					}
				}    
			);

		#endregion

		#region Public Methods

		/// <summary>
		/// Retrieves the an available policy builder using the provided policy name.
		/// </summary>
		/// <param name="policyName">The name of the policy to retrieve.</param>
		/// <returns>The policy builder used for the policy name, or an empty function if not found.</returns>
		public static Action<AuthorizationPolicyBuilder> GetPolicy(PolicyNames policyName)
		{
			var policyNames = Enum.GetValues(typeof(PolicyNames))
				.Cast<PolicyNames>()
				.ToList();

			return builder =>
			{
				foreach (var policyNameValue in policyNames)
				{
					if (AvailablePolicies.TryGetValue(policyNameValue & policyName, out var policyBuilder))
					{
						policyBuilder(builder);
					}
				}
			};
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Check if a claim type exists in the identity.
		/// </summary>
		/// <param name="builder">Builder for the authorization policy.</param>
		/// <param name="claimType">The type of claim to check for.</param>
		/// <param name="validate">Additional validation rules for the value of the claim.</param>
		private static void CheckClaim(
			AuthorizationPolicyBuilder builder, 
			string claimType, 
			Func<string, bool> validate = null
		)
		{
			builder.RequireAssertion
			(
				context => context.User.HasClaim
				(
					claim => claim.Type == claimType && (validate == null || validate(claim.Value))
				)
			);
		}

		#endregion
	}
}
