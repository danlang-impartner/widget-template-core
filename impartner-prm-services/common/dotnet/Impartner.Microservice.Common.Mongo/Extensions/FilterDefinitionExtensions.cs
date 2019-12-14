using System;
using System.Linq.Expressions;
using MongoDB.Driver;
using TenantDocument = Impartner.Microservice.Common.Mongo.Models.TenantDocument.V0;

namespace Impartner.Microservice.Common.Mongo.Extensions
{
	public static class FilterDefinitionExtensions
	{
		#region Fields

		private const int AdminTenant = -1;

		#endregion

		#region Internal Methods

		/// <summary>
		/// Adds an additional check for the given tenant id. This should apply the current filter to all results that are available to the current user.
		/// </summary>
		/// <typeparam name="T">The type of document that is being filtered in the database. It is required to be a <see cref="TenantDocument"/> type.</typeparam>
		/// <param name="definition">The current document filter definition that should be applied to the collection.</param>
		/// <param name="tenantId">The ID of the tenant to check against. If the tenant id is -1, no tenant restriction will be applied.</param>
		/// <returns>An updated filter that only filters against items that are available to the user.</returns>
		internal static FilterDefinition<T> AddTenantFilter<T>(this FilterDefinition<T> definition, int tenantId)
			where T : TenantDocument =>
			tenantId != AdminTenant ?
				definition & Builders<T>.Filter.Eq(nameof(tenantId), tenantId) :
				definition;

		/// <summary>
		/// Adds an additional check for the given tenant id. This should apply the current filter to all results that are available to the current user.
		/// </summary>
		/// <typeparam name="T">The type of document that is being filtered in the database. It is required to be a <see cref="TenantDocument"/> type.</typeparam>
		/// <param name="filter">The current filter function that should be applied to the collection.</param>
		/// <param name="tenantId">The ID of the tenant to check against. If the tenant id is -1, no tenant restriction will be applied.</param>
		/// <returns>An updated filter that only filters against items that are available to the user.</returns>
		internal static FilterDefinition<T> AddTenantFilter<T>(this Expression<Func<T, bool>> filter, int tenantId)
			where T : TenantDocument => AddTenantFilter(Builders<T>.Filter.Where(filter), tenantId);

		#endregion
	}
}
