namespace Impartner.Microservice.Common.Mongo.Models
{
	/// <summary>
	/// Settings for interacting with MongoDB.
	/// </summary>
	public class MongoDbSettings
	{
		#region Properties

		/// <summary>
		/// Connection string to the mongo database.
		/// </summary>
		public string ConnectionString { get; set; }

		/// <summary>
		/// Name of the database used by this microservice. A Microservice should only have one database.
		/// </summary>
		public string DatabaseName { get; set; }

		#endregion
	}
}
