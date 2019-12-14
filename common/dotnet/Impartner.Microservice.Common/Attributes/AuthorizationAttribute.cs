using Impartner.Microservice.Common.Authorization;
using BaseAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;

namespace Impartner.Microservice.Common.Attributes
{
	/// <summary>
	/// Attribute extension that adds the ability to use Policy Names
	/// </summary>
	public class AuthorizeAttribute : BaseAttribute
	{
		#region Constructors

		/// <summary>
		/// Default Constructor.
		/// Requires authenticated token by the controller or endpoint that is decorated by this attribute.
		/// </summary>
		public AuthorizeAttribute() {}

		/// <summary>
		/// Wrapper constructor for base <see cref="Microsoft.AspNetCore.Authorization.AuthorizeAttribute"/> with a policy.
		/// Requires authenticated token that passes the policy rules by the controller or endpoint
		/// that is decorated by this attribute.
		/// </summary>
		/// <param name="policy">The name of the built in policies to use.</param>
		public AuthorizeAttribute(string policy) : base(policy) {}

		/// <summary>
		/// Attribute that takes a <see cref="PolicyNames" /> to authorize against the current identity.
		/// Requires authenticated token that passes the policy rules by the controller or endpoint
		/// that is decorated by this attribute.
		/// </summary>
		/// <param name="policy">The name of the built in policies to use.</param>
		public AuthorizeAttribute(PolicyNames policy) 
			: this(policy.ToString()) { }

		#endregion
	}
}
