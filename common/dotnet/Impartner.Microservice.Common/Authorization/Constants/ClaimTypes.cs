namespace Impartner.Common.Security.Constants
{
	// TODO - Temporary port until Impartner.Common project becomes a NuGetPackage.

	/// <summary>
	/// Collection of common claim types.
	/// This collection may not contain all claims that we use, but these are the common claim types that can be shared across projects.
	/// </summary>
	public static class ClaimTypes
	{
		#region Fields

		/// <summary>
		/// Type of claim that stores the ID of the company the user is associated with, that is doing business with the tenant.
		/// </summary>
		public const string AccountId = "urn:impartner:account-id";

		/// <summary>
		/// Type of claim that stores the ID of the customer organization the user is associated with.
		/// </summary>
		public const string TenantId = "urn:impartner:tenant-id";

		/// <summary>
		/// Type of claim that stores the ID of the user.
		/// </summary>
		public const string UserId = "urn:impartner:user-id";

		/// <summary>
		/// Type of claim that stores the type of user.
		/// </summary>
		public const string UserType = "urn:impartner:user-type";

		#endregion
	}
}
