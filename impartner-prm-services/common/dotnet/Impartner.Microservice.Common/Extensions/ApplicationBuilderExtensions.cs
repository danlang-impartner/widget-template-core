using System;
using Impartner.Microservice.Common.Models;
using Impartner.Microservice.Common.Models.Responses;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Impartner.Microservice.Common.Extensions
{
	/// <summary>
	/// Collection of functions that configure the application.
	/// </summary>
	public static class ApplicationBuilderExtensions
	{
		#region Public Methods

		/// <summary>
		/// Uses a response writer to create a detailed response for each health check that was ran.
		/// </summary>
		/// <param name="builder">Application builder that constructs the service pipeline.</param>
		/// <param name="route">The route to the health check; By default, the route is '/healthcheck'.</param>
		/// <param name="allowCachingResponses">Whether to allow cached responses.</param>
		/// <param name="predicate">Rules for which health checks are accessed by this endpoint.</param>
		public static IApplicationBuilder UseDetailedHealthChecks(
			this IApplicationBuilder builder,
			string route = "/healthcheck",
			bool allowCachingResponses = false,
			Func<HealthCheckRegistration, bool> predicate = null
		)
		{
			builder.UseHealthChecks(route, new HealthCheckOptions
			{
				AllowCachingResponses = allowCachingResponses,
				Predicate = predicate ?? (healthCheck => true),
				ResponseWriter = (context, report) =>
				{
					var healthCheckResponse = new DetailedHealthCheckResponse(report);

					return context.Response.WriteAsync(healthCheckResponse.ToString());
				}
			});

			return builder;
		}

		/// <summary>
		/// Adds Swagger and Swagger UI using the base Impartner configuration.
		/// </summary>
		/// <param name="builder">Application builder that constructs the service pipeline.</param>
		/// <param name="serviceInfo">
		/// Information about the service, including title and version, used to configure the Swagger endpoints.
		/// </param>
		public static IApplicationBuilder UseSwagger(
			this IApplicationBuilder builder,
			params ServiceInfo[] serviceInfo
		) => UseSwagger(builder, null, null, serviceInfo);

		public static IApplicationBuilder UseSwagger(
			this IApplicationBuilder builder,
			Action<SwaggerOptions> configureSwaggerOptions = null,
			Action<SwaggerUIOptions> configureSwaggerUiOptions = null,
			params ServiceInfo[] serviceInfo
		)
		{
			SwaggerBuilderExtensions.UseSwagger(builder, configureSwaggerOptions);
			builder.UseSwaggerUI(options =>
			{
				foreach (var info in serviceInfo)
				{
					options.SwaggerEndpoint
					(

						$"../swagger/{info.Version.ToLower()}/swagger.json",
						$"{info.Title} {info.Version.ToUpper()}"
					);
				}

				configureSwaggerUiOptions?.Invoke(options);
			});
			return builder;
		}

		#endregion
	}
}
