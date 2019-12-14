using System.Collections.Generic;
using Newtonsoft.Json;

namespace Impartner.Microservice.Common.Models.Responses
{
	/// <summary>
	/// An individual health check result for the detailed health check.
	/// </summary>
	public class DetailedHealthCheckResponseItem
	{
		#region Properties

		/// <summary>
		/// Collection of data the health check included in its result.
		/// </summary>
		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public IDictionary<string, object> Data { get; set; }

		/// <summary>
		/// Description of the result of the health check.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Any exception details from exceptions that were thrown by the health check.
		/// </summary>
		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public ApiError[] Errors { get; set; }

		/// <summary>
		/// The status of this health check.
		/// </summary>
		public string Status { get; set; }

		#endregion
	}
}
