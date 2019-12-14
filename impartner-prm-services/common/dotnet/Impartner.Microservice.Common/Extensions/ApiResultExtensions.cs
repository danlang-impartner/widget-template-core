using Impartner.Microservice.Common.Models.Responses;

namespace Impartner.Microservice.Common.Extensions
{
	/// <summary>
	/// Collection of extensions related to <see cref="ApiResult"/> or <see cref="ApiResult{TData}"/>
	/// </summary>
	public static class ApiResultExtensions
	{
		#region Public Methods

		/// <summary>
		/// wraps the value in an ApiResult envelope.
		/// </summary>
		/// <typeparam name="TData">The type of data that is being wrapped in an ApiResult.</typeparam>
		/// <param name="value">The data that will be placed in the <see cref="ApiResult.Data"/> property.</param>
		/// <param name="errors">Collection of errors related to the result of the API request.</param>
		/// <returns>An ApiResult wrapping the provided value and its errors.</returns>
		public static ApiResult<TData> ToApiResult<TData>(this TData value, params ApiError[] errors)
		{
			return new ApiResult<TData>
			{
				Data = value,
				Errors = errors
			};
		}

		#endregion
	}
}
