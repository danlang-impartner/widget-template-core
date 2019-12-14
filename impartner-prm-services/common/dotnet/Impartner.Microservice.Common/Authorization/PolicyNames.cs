using System;

namespace Impartner.Microservice.Common.Authorization
{
	/// <summary>
	/// Policy names that map to a authorization policy. 
	/// </summary>
	[Flags] 
	public enum PolicyNames
	{
		AccountId = 1 << 0,
		TenantId = 1 << 1,
		UserId = 1 << 2,
		IsAdmin = 1 << 3,
		EnvironmentContext = 1 << 4
	}
}
