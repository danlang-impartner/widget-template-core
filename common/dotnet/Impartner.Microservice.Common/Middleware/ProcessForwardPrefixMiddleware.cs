using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Impartner.Microservice.Common.Middleware
{
	/// <summary>
	/// Process forward prefix header by moving the value into the context request.
	/// </summary>
	public class ProcessForwardPrefixMiddleware
	{
		#region Fields

		/// <summary>
		/// Service for logging messages.
		/// </summary>
		private readonly ILogger<ProcessForwardPrefixMiddleware> _logger;

		/// <summary>
		/// The delegate representing the remaining middleware in the request pipeline.
		/// </summary>
		private readonly RequestDelegate _next;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the Prefix middleware.
		/// </summary>
		/// <param name="next">The delegate representing the remaining middleware in the request pipeline.</param>
		/// <param name="logger">Service for logging messages.</param>
		public ProcessForwardPrefixMiddleware(RequestDelegate next, ILogger<ProcessForwardPrefixMiddleware> logger)
		{
			_next = next ?? throw new ArgumentNullException(nameof(next));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		#endregion

		#region Public Methods

		/// <summary>Request handling method.</summary>
		/// <param name="context">The <see cref="T:Microsoft.AspNetCore.Http.HttpContext" /> for the current request.</param>
		/// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the execution of this middleware.</returns>
		public async Task InvokeAsync(HttpContext context)
		{
			_logger.LogDebug($"request_url: {context.Request.Path}");
			var forwardedPath = context.Request.Headers["X-Forwarded-Prefix"].FirstOrDefault();
			if (!string.IsNullOrEmpty(forwardedPath))
			{
				_logger.LogDebug($"forwardedPath:{forwardedPath}");
				context.Request.PathBase = forwardedPath;
			}

			await _next(context);
		}

		#endregion
	}
}
