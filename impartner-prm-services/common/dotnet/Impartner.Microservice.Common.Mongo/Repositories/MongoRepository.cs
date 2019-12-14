using Impartner.Microservice.Common.Extensions;
using Impartner.Microservice.Common.Mongo.Extensions;
using Impartner.Microservice.Common.Mongo.Models;
using Impartner.Microservice.Common.Utilities;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TenantDocument = Impartner.Microservice.Common.Mongo.Models.TenantDocument.V0;

namespace Impartner.Microservice.Common.Mongo.Repositories
{
	public interface IMongoRepository
	{
		#region Public Methods

		Task<DeleteResult> DeleteAsync<T>(string collection, FilterDefinition<T> filter) where T : TenantDocument;
		Task<DeleteResult> DeleteAsync<T>(string collection, Expression<Func<T, bool>> filter) where T : TenantDocument;
		Task<DeleteResult> DeleteManyAsync<T>(string collection, Expression<Func<T, bool>> filter) where T : TenantDocument;
		IFindFluent<T, T> Find<T>(string collection, FilterDefinition<T> filter) where T : TenantDocument;
		IFindFluent<T, T> Find<T>(string collection, Expression<Func<T, bool>> filter) where T : TenantDocument;
		Task<IAsyncCursor<T>> FindAsync<T>(string collection, Expression<Func<T, bool>> filter) where T : TenantDocument;
		IMongoCollection<T> GetCollection<T>(string collection) where T : TenantDocument;
		IMongoDatabase GetDatabase();
		Task<T> SaveAsync<T>(string collection, T data) where T : TenantDocument;
		Task<ReplaceOneResult> UpdateAsync<T>(string collection, FilterDefinition<T> filter, T data) where T : TenantDocument;
		Task<ReplaceOneResult> UpdateAsync<T>(string collection, Expression<Func<T, bool>> filter, T data) where T : TenantDocument;
		Task<UpdateResult> UpdateManyAsync<T>(string collection, Expression<Func<T, bool>> filter, UpdateDefinition<T> updateDef, bool isUpsert = false) where T : TenantDocument;

		#endregion
	}

	public class MongoRepository : IMongoRepository
	{
		#region Fields

		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IMongoClient _mongoClient;
		private readonly MongoDbSettings _mongoDbSettings;

		/// <summary>
		/// Regex will truncate a string to 64 characters. Necessary for MongoDB database names.
		/// </summary>
		private readonly Regex _truncateDatabaseNameRegex = StringUtilities.CreateTruncateRegex(max: 64);

		#endregion

		#region Properties

		/// <summary>
		/// Name of the database to access when performing database actions.
		/// 
		/// This value combines the database name from the config and the environmental context from the request, removing all invalid characters and applying 
		/// </summary>
		private string DatabaseName => Regex.Replace
			(
				// Reduce the string to no more then 64 characters, to meet Mongo requirements.
				StringUtilities.ApplyTruncateRegex($"{_mongoDbSettings.DatabaseName}_{_httpContextAccessor.GetEnvironmentContext()}", _truncateDatabaseNameRegex),
				// Remove all characters that are considered unacceptable.
				@"[\/\\. ""$*<>:|?]",
				""
			)
			.ToLowerInvariant();

		private int TenantId => _httpContextAccessor.GetTenantId();

		#endregion

		#region Constructors

		public MongoRepository(IMongoClient mongoClient, IHttpContextAccessor httpContextAccessor, MongoDbSettings mongoDbSettings)
		{
			_mongoClient = mongoClient ?? throw new ArgumentNullException(nameof(mongoClient));
			_httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
			_mongoDbSettings = mongoDbSettings ?? throw new ArgumentNullException(nameof(mongoDbSettings));
		}

		#endregion

		#region Public Methods

		public Task<DeleteResult> DeleteAsync<T>(string collection, FilterDefinition<T> filter)
			where T : TenantDocument
		{
			return GetCollection<T>(collection).DeleteOneAsync(filter.AddTenantFilter(TenantId));
		}

		public Task<DeleteResult> DeleteAsync<T>(string collection, Expression<Func<T, bool>> filter)
			where T : TenantDocument
		{
			return GetCollection<T>(collection).DeleteOneAsync(filter.AddTenantFilter(TenantId));
		}

		public Task<DeleteResult> DeleteManyAsync<T>(string collection, Expression<Func<T, bool>> filter)
			where T : TenantDocument
		{
			return GetCollection<T>(collection).DeleteManyAsync(filter);
		}

		public IFindFluent<T, T> Find<T>(string collection, FilterDefinition<T> filter)
			where T : TenantDocument
		{
			return GetCollection<T>(collection).Find(filter.AddTenantFilter(TenantId));
		}

		public IFindFluent<T, T> Find<T>(string collection, Expression<Func<T, bool>> filter)
			where T : TenantDocument
		{
			return GetCollection<T>(collection).Find(filter.AddTenantFilter(TenantId));
		}

		public Task<IAsyncCursor<T>> FindAsync<T>(string collection, Expression<Func<T, bool>> filter)
			where T : TenantDocument
		{
			return GetCollection<T>(collection).FindAsync(filter.AddTenantFilter(TenantId));
		}

		public IMongoCollection<T> GetCollection<T>(string collection)
			where T : TenantDocument
		{
			return GetDatabase().GetCollection<T>(collection);
		}

		public IMongoDatabase GetDatabase()
		{
			return _mongoClient.GetDatabase(DatabaseName);
		}

		public async Task<T> SaveAsync<T>(string collection, T data)
			where T : TenantDocument
		{
			data.TenantId = TenantId;
			await GetCollection<T>(collection).InsertOneAsync(data);
			return data;
		}

		public Task<ReplaceOneResult> UpdateAsync<T>(string collection, FilterDefinition<T> filter, T data)
			where T : TenantDocument
		{
			return GetCollection<T>(collection).ReplaceOneAsync(filter.AddTenantFilter(TenantId), data);
		}

		public Task<ReplaceOneResult> UpdateAsync<T>(string collection, Expression<Func<T, bool>> filter, T data)
			where T : TenantDocument
		{
			return GetCollection<T>(collection).ReplaceOneAsync(filter.AddTenantFilter(TenantId), data);
		}

		public Task<UpdateResult> UpdateManyAsync<T>(string collection, Expression<Func<T, bool>> filter, UpdateDefinition<T> updateDef, bool isUpsert = false)
			where T : TenantDocument
		{
			return GetCollection<T>(collection).UpdateManyAsync(filter, updateDef, new UpdateOptions { IsUpsert = isUpsert });
		}

		#endregion
	}
}
