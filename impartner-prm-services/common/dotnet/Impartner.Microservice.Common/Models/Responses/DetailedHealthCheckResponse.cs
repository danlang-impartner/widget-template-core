using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;

namespace Impartner.Microservice.Common.Models.Responses
{
	/// <summary>
	/// Response given when requesting a detailed health check.
	/// </summary>
	public class DetailedHealthCheckResponse
	{
		#region Properties

		/// <summary>
		/// Mapping of health check names to their results.
		/// </summary>
		public IDictionary<string, DetailedHealthCheckResponseItem> Results { get; set; }
			= new Dictionary<string, DetailedHealthCheckResponseItem>();

		/// <summary>
		/// Overall status of the health check; This will be the worst result of all health checks ran.
		/// </summary>
		public string Status { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs a response using the health report.
		/// </summary>
		/// <param name="healthReport">The full health check report.</param>
		public DetailedHealthCheckResponse(HealthReport healthReport)
		{
			Status = healthReport.Status.ToString();

			// Represent each entry from the report as a item in the Results dictionary.
			foreach (var (key, value) in healthReport.Entries)
			{
				Results[key] = new DetailedHealthCheckResponseItem
				{
					Data = new Dictionary<string, object>(value.Data),
					Description = value.Description,
					Status = value.Status.ToString()
				};

				// The aggregate exception is a collection of many exceptions, so bring that collection to the top.
				if (value.Exception is AggregateException aggregateException)
				{
					Results[key].Errors = aggregateException.InnerExceptions
						.Select(exception => new ApiError(exception.GetType().Name, exception, ApiErrorSeverity.Error))
						.ToArray();
				}
				// Otherwise, record the single exception.
				else if (value.Exception != null)
				{
					Results[key].Errors = new []
					{
						new ApiError(value.Exception.GetType().Name, value.Exception, ApiErrorSeverity.Error)
					};
				}
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Converts the object to a JSON representation so that it can given as a JSON response.
		/// </summary>
		/// <returns>JSON representation of this object.</returns>
		public override string ToString() => JsonConvert.SerializeObject(this);

		#endregion
	}
}
