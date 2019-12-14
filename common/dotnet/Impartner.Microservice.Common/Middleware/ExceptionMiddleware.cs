using System;
using System.Net;
using System.Threading.Tasks;
using Impartner.Microservice.Common.Exceptions;
using Impartner.Microservice.Common.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;

namespace Impartner.Microservice.Common.Middleware
{
	/// <summary>
	/// Middleware that will convert an exception from the service into a reasonable JSON response.
	/// </summary>
	public class ExceptionMiddleware
	{
		#region Fields

		/// <summary>
		/// Service for logging messages.
		/// </summary>
		private readonly ILogger<ExceptionMiddleware> _logger;

		/// <summary>
		/// The delegate representing the remaining middleware in the request pipeline.
		/// </summary>
		private readonly RequestDelegate _next;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the middleware with the next item in the pipeline.
		/// </summary>
		/// <param name="next">Next middleware delegate in the pipeline.</param>
		/// <param name="logger">Service for logging messages.</param>
		public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
		{
			_next = next ?? throw new ArgumentNullException(nameof(next));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Wraps the pipeline with a Try/Catch to provide error safety.
		/// Errors are turned into an HTTP Response and returned.
		/// </summary>
		/// <param name="httpContext">The current HTTP Context being processed.</param>
		public async Task InvokeAsync(HttpContext httpContext)
		{
			try
			{
				await _next(httpContext);
			}
			catch (Exception ex)
			{
				_logger.LogCritical(ex, $"Something went wrong and was captured by {nameof(ExceptionMiddleware)}");
				await HandleExceptionAsync(httpContext, ex);
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Converts the provided exception to an HTTP response.
		/// </summary>
		/// <param name="httpContext">The current HTTP Context being processed.</param>
		/// <param name="exception">The exception that was thrown and needs to be converted to an HTTP Response.</param>
		private Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
		{
			httpContext.Response.ContentType = "application/json";
			httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

			_logger.LogCritical
			(
				exception,
				"There was a failure while executing {Method} {@Request}",
				httpContext.Request.Method,
				httpContext.Request.GetDisplayUrl()
			);

			if (!(exception is ImpartnerException impartnerException))
			{
				return httpContext.Response.WriteAsync
				(
					new ApiResult
					{
						Message = $"An unknown exception occured: {exception.Message}",
						Errors = new []{ new ApiError("UnknownException", exception) }
					}.ToString()
				);
			}

			httpContext.Response.StatusCode = (int) impartnerException.StatusCode;

			return httpContext.Response.WriteAsync
			(
				new ApiResult
				{
					Message = impartnerException.Message,
					Errors = impartnerException.Errors
				}.ToString()
			);

		}

		#endregion
	}
}
