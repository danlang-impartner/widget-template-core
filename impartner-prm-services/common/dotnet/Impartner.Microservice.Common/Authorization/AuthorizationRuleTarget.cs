using System;

namespace Impartner.Common.Security
{
	// TODO - Temporary port until Impartner.Common project becomes a NuGetPackage.

	/// <summary>The permitted target of an authorization rule</summary>
	[Flags]
	public enum AuthorizationRuleTarget
	{
		// NOTE!!!
		// The ordering of these is important. A higher privileged target level must have a 
		// higher int value than a lower privileged target. 

		/// <summary>None</summary>
		None = 0,
		/// <summary>Anonymous users</summary>
		Anon = 1,
		/// <summary>Members</summary>
		Member = 2,
		/// <summary>Administrators</summary>
		Admin = 4,
		/// <summary>TreeHouse Super Users</summary>
		Super = 8,
		/// <summary>All</summary>
		All = Anon | Member | Admin | Super
	}
}
