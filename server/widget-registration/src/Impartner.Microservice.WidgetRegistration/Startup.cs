using Amazon;
using Amazon.S3;
using Impartner.Microservice.Common.Authorization;
using Impartner.Microservice.Common.Extensions;
using Impartner.Microservice.Common.Middleware;
using Impartner.Microservice.Common.Models;
using Impartner.Microservice.Common.Mongo.Extensions;
using Impartner.Microservice.WidgetRegistration.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Impartner.Microservice.WidgetRegistration
{
	public class Startup
	{
		#region Properties

		/// <summary>
		/// Configuration loaded from appsettings.json and Environment Variables.
		/// </summary>
		public IConfiguration Configuration { get; }

		/// <summary>
		/// Information about this service that will be used to generate Swagger documentation for the service.
		/// </summary>
		public ServiceInfo ServiceInfo { get; } =
			new ServiceInfo { Title = "Widget Registration", Version = "V1" };

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the application, loading the configuration loaded from appsettings.json and Environment Variables.
		/// </summary>
		/// <param name="configuration">Configuration loaded from appsettings.json and Environment Variables.</param>
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Configures the HTTP request pipeline.
		/// This method gets called by the runtime.
		/// </summary>
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseHsts();
			}

			app.UseSerilogRequestLogging();

			app.UseAuthentication();
			app.UseForwardedHeaders(new ForwardedHeadersOptions
			{
				ForwardedHeaders = ForwardedHeaders.All
			});

			app.UseMiddleware<ExceptionMiddleware>();
			// Patch path base with forwarded path
			app.UseMiddleware<ProcessForwardPrefixMiddleware>();

			app.UseDetailedHealthChecks();

			//app.UseHttpsRedirection();
			// Enable middleware to serve generated Swagger as a JSON endpoint and Swagger UI.
			app.UseSwagger(ServiceInfo);
			app.UseMvc();
		}

		/// <summary>
		/// Add services to the container.
		/// This method gets called by the runtime.
		/// </summary>
		public void ConfigureServices(IServiceCollection services)
		{
			services
				.AddImpartner
				(
					Configuration,
					builder => builder.AddMongoDbHealthChecks()
				)
				.AddAuthentication()
				.AddAuthorization(
					PolicyNames.TenantId,
					PolicyNames.IsAdmin
				)
				.AddMongo()
				.AddCors()
				.AddMvc()
				.AddSwaggerGen(ServiceInfo);

			var awsSettings = Configuration.GetSection("AWSSettings");
			services.AddScoped<IAmazonS3, AmazonS3Client>(x =>
			{
				var awsAccessKeyId = awsSettings["awsAccessKeyId"];
				var awsSecretAccessKey = awsSettings["awsSecretAccessKey"];
				return new AmazonS3Client(awsAccessKeyId, awsSecretAccessKey,
					RegionEndpoint.USWest2);
			});
			services.AddScoped<ICdnFileSaver, CdnFileSaver>();
			services.AddScoped<IManifestValidator, ManifestValidator>();
		}

		#endregion
	}
}
