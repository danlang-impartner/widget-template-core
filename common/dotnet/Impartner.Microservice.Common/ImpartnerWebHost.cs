using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;
using Serilog.Events;

namespace Impartner.Microservice.Common
{
	public static class ImpartnerWebHost
	{
		public static IWebHostBuilder CreateWebHostBuilder<TStartup>(
			string[] args, 
			Action<KestrelServerOptions> configureKestrel = null,
			Action<WebHostBuilderContext, LoggerConfiguration> configureLogger = null,
			Action<IWebHostBuilder> configureWebHost = null
		) where TStartup : class 
		{
			var webHost = WebHost
				.CreateDefaultBuilder<TStartup>(args)
				.UseKestrel(options => configureKestrel?.Invoke(options))
				.UseSerilog
				(
					(context, loggerConfiguration) =>
					{
						const string outputTemplate = "{Level} {Timestamp:HH:mm:ss} {Message:lj}{NewLine}{Exception}";

						loggerConfiguration
							.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
							.Enrich.FromLogContext()
							.WriteTo.Console(outputTemplate: outputTemplate)
							.WriteTo.Async
							(
								sink => sink.File
								(
									@"Logs/logfile.log",
									outputTemplate: outputTemplate,
									rollingInterval: RollingInterval.Day
								)
							)
							.ReadFrom.Configuration(context.Configuration, "Logging");
                        
						configureLogger?.Invoke(context, loggerConfiguration);
					}
				);
			configureWebHost?.Invoke(webHost);

			return webHost;
		}
	}
}
