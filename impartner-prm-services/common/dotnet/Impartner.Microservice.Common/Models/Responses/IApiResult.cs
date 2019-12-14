namespace Impartner.Common.Api
{
	// TODO - Remove when Impartner.Common is a NuGet Package

	/// <summary>
	/// A model representing the results of the api. Provides
	/// </summary>
	public interface IApiResult
	{
		#region Properties

		/// <summary>
		/// Collection of errors associated with the API.
		/// </summary>
		IApiError[] Errors { get; set; }

		/// <summary>
		/// A top level summary message of the status of this result.
		/// </summary>
		string Message { get; set; }

		#endregion
	}

	/// <summary>
	/// Api result that contains a data result.
	/// </summary>
	/// <typeparam name="T">The type used for the result. Can be a collection, object or a basic type.</typeparam>
	public interface IApiResult<T> : IApiResult
	{
		#region Properties

		/// <summary>
		/// The data result of the response.
		/// </summary>
		T Data { get; set; }

		#endregion
	}
}
