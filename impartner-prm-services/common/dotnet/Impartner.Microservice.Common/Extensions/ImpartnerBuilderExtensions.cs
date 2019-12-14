using System;
using System.Collections.Generic;
using System.Linq;
using Impartner.Microservice.Common.Authentication;
using Impartner.Microservice.Common.Authorization;
using Impartner.Microservice.Common.Exceptions.Http;
using Impartner.Microservice.Common.Models;
using Impartner.Microservice.Common.Models.Responses;
using Impartner.Microservice.Common.Swagger.OperationFilters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Impartner.Microservice.Common.Extensions
{
	/// <summary>
	/// Collection of functions that configure the Impartner Service and its dependencies.
	/// </summary>
	public static class ImpartnerBuilderExtensions
	{
		#region Public Methods

		/// <summary>
		/// Adds authentication with Impartner settings.
		/// </summary>
		/// <param name="builder">Builder for adding the service into the Impartner ecosystem.</param>
		/// <param name="configureOptions">Action to add additional configuration to the Authentication settings.</param>
		/// <returns>Builder for adding the service into the Impartner ecosystem.</returns>
		public static IImpartnerBuilder AddAuthentication(
			this IImpartnerBuilder builder, 
			Action<JwtBearerOptions> configureOptions = null
		) 
		{
			builder.ServiceCollection.AddAuthentication("Bearer")
				.AddJwtBearer(options =>
				{
					options.Audience = builder.Configuration["Authentication:ClientId"];
					options.SaveToken = true;

					options.TokenValidationParameters = new TokenValidationParameters
					{
						// TODO - We aren't hosting a `.well-known/openid-configuration` route,
						// TODO - so disable issuer and audience until we can figure out what to do about it.
						RequireSignedTokens = false,
						ValidateAudience = false,
						ValidateIssuer = false
					};

					options.Events = new JwtBearerEvents
					{
						OnAuthenticationFailed = AuthenticationEvents.AuthenticationFailed,
						OnTokenValidated = AuthenticationEvents.TokenValidated
					};

					configureOptions?.Invoke(options);
				});
			return builder;
		}

		/// <summary>
		/// Adds authorizations with built-in policies.
		/// </summary>
		/// <param name="builder">Builder for adding the service into the Impartner ecosystem.</param>
		/// <param name="policies">Collection of policies that should be added to the authorization</param>
		/// <returns>Builder for adding the service into the Impartner ecosystem.</returns>
		public static IImpartnerBuilder AddAuthorization(
			this IImpartnerBuilder builder, 
			params PolicyNames[] policies
		) => AddAuthorization(builder, null, policies);

		/// <summary>
		/// Adds authorizations with built-in policies.
		/// </summary>
		/// <param name="builder">Builder for adding the service into the Impartner ecosystem.</param>
		/// <param name="configureOptions">Action to add additional configuration to the Authorization settings.</param>
		/// <param name="policies">Collection of policies that should be added to the authorization</param>
		/// <returns>Builder for adding the service into the Impartner ecosystem.</returns>
		public static IImpartnerBuilder AddAuthorization(
			this IImpartnerBuilder builder, 
			Action<AuthorizationOptions> configureOptions, 
			params PolicyNames[] policies
		)
		{
			builder.ServiceCollection.AddAuthorization(
				options =>
				{
					foreach (var policy in policies)
					{
						options.AddPolicy(
							policy.ToString(), 
							Policy.GetPolicy(policy)
						);
					}

					configureOptions?.Invoke(options);
				}
			);
			return builder;
		}

		/// <summary>
		/// Adds CORS with Impartner settings to a service.
		/// </summary>
		/// <param name="builder">Builder for adding the service into the Impartner ecosystem.</param>
		/// <param name="configureOptions">Action to add additional configuration to the CORS settings.</param>
		/// <returns>Builder for adding the service into the Impartner ecosystem.</returns>
		public static IImpartnerBuilder AddCors(this IImpartnerBuilder builder, Action<CorsOptions> configureOptions = null)
		{
			builder.ServiceCollection.AddCors(options =>
			{
				// TODO - Add CORS Impartner ecosystem specific setup here.
				configureOptions?.Invoke(options);
			});

			return builder;
		}

		/// <summary>
		/// Add MVC with Impartner settings to a service; more specifically, JSON format settings.
		/// </summary>
		/// <param name="builder">Builder for adding the service into the Impartner ecosystem.</param>
		/// <param name="configureOptions">Action to add additional configuration to the MVC settings.</param>
		/// <param name="configureApiBehaviorOptions"></param>
		/// <param name="configureJsonOptions">Action to add additional configuration to the JSON settings.</param>
		/// <param name="configureMvc">Action to add additional configuration to the MVC builder.</param>
		/// <returns>Builder for adding the service into the Impartner ecosystem.</returns>
		public static IImpartnerBuilder AddMvc(
			this IImpartnerBuilder builder, 
			Action<MvcOptions> configureOptions = null,
			Action<ApiBehaviorOptions> configureApiBehaviorOptions = null,
			Action<MvcJsonOptions> configureJsonOptions = null,
			Action<IMvcBuilder> configureMvc = null
		)
		{
			var mvcBuilder = builder.ServiceCollection
				.AddMvc(options =>
				{
					configureOptions?.Invoke(options);
				})
				.ConfigureApiBehaviorOptions(options =>
				{
					options.InvalidModelStateResponseFactory =
						context => throw new HttpBadRequestException
						(
							context.ModelState
								.Where(keyValuePair => keyValuePair.Value.Errors.Any())
								.Select(keyValuePair => new ApiError
									(
										"InvalidModelState",
										"The attempted value(s), {attemptedValues}, was not valid for the property {property}. Validation errors: {errors}",
										additionalData: new Dictionary<string, object>
										{
											{"attemptedValues", keyValuePair.Value.AttemptedValue},
											{"property", keyValuePair.Key},
											{
												"errors",
												string.Join(", ",
													keyValuePair.Value.Errors.Select(error => error.ErrorMessage))
											}
										}
									)
								)
								.ToArray()
						);

					configureApiBehaviorOptions?.Invoke(options);
				})
				.AddJsonOptions(options =>
				{
					options.SerializerSettings.Formatting = Formatting.Indented;
					options.SerializerSettings.ContractResolver = new DefaultContractResolver
					{
						NamingStrategy = new CamelCaseNamingStrategy
						{
							ProcessDictionaryKeys = false,
							OverrideSpecifiedNames = true
						}
					};
					options.SerializerSettings.Converters.Add(new StringEnumConverter{CamelCaseText = false});
					configureJsonOptions?.Invoke(options);

					JsonConvert.DefaultSettings = () => options.SerializerSettings;
				})
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

			configureMvc?.Invoke(mvcBuilder);

			return builder;
		}

		public static IImpartnerBuilder AddSwaggerGen(
			this IImpartnerBuilder builder,
			params ServiceInfo[] serviceInfo
		) => AddSwaggerGen(builder, null, serviceInfo);

		public static IImpartnerBuilder AddSwaggerGen(
			this IImpartnerBuilder builder,
			Action<SwaggerGenOptions> configureOptions = null,
			params ServiceInfo[] serviceInfo
		)
		{
			builder.ServiceCollection.AddSwaggerGen(options =>
			{
				foreach (var info in serviceInfo)
				{
					options.SwaggerDoc(info.Version.ToLower(), info);
				}
                
				options.AddSecurityDefinition("Bearer", new ApiKeyScheme
				{
					Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
					Name = "Authorization",
					In = "header",
					Type = "apiKey",
				});

				options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
					{ "Bearer", Enumerable.Empty<string>() },
				});

				options.OperationFilter<EnvironmentContextHeaderFilter>();
				options.EnableAnnotations();

				configureOptions?.Invoke(options);
			});

			return builder;
		}

		#endregion
	}
}
