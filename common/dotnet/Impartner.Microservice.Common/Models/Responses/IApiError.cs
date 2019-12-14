namespace Impartner.Common.Api
{
	// TODO - Remove when Impartner.Common is a NuGet Package

	/// <summary>
	/// Represents an error produced by the 
	/// </summary>
	public interface IApiError
	{
		#region Properties

		/// <summary>
		/// The error code; This should be used for localization of the error message.
		/// </summary>
		string Code { get; set; }

		/// <summary>
		/// Details of the error. This should be localized to the current API, but may be localized differently on the client.
		/// </summary>
		string Message { get; set; }

		#endregion
	}
}
