using Amazon.S3;
using Amazon.S3.Model;
using Impartner.Microservice.Common.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Impartner.Microservice.Training.Services
{
	public interface IMessenger
	{
		Task<bool> SendMessage(string objectName, object data);
	}

	public class Messenger : IMessenger
	{
		public const string BucketRoot = "ms2prm-msg-";
		public const string ObjectNameMetaField = "datatype";
		private readonly IAmazonS3 _s3Client;
		private readonly ILogger _logger;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public string BucketName
		{
			get
			{
				string awsEnvName;
				var envContext = _httpContextAccessor.GetEnvironmentContext()?.ToLowerInvariant();
				switch (envContext)
				{
					case "development":
						awsEnvName = "dev";
						break;
					case "staging":
						awsEnvName = "stage";
						break;
					case "production":
						awsEnvName = "prod";
						break;
					default:
						awsEnvName = envContext.Substring(0, 24);
						break;
				}
				return $"{BucketRoot}{awsEnvName}";
			}
		}

		public Messenger(IAmazonS3 s3Client, ILogger<Messenger> logger, IHttpContextAccessor httpContextAccessor)
		{
			_s3Client = s3Client;
			_logger = logger;
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<bool> SendMessage(string objectName, object data)
		{
			try
			{
				var request = new PutObjectRequest
				{
					BucketName = BucketName,
					Key = $"{objectName}/{Guid.NewGuid()}.json",
					InputStream = data.ToStream()
				};
				request.Metadata.Add(ObjectNameMetaField, objectName);
				var response = await _s3Client.PutObjectAsync(request);
				return true;
			}
			catch (Exception e)
			{
				_logger.Log(LogLevel.Error, e, $"Error saving file to bucket: {BucketName}");
				return false;
			}
		}
	}
}
