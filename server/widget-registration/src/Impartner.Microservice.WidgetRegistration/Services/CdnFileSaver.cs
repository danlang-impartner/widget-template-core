using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Impartner.Microservice.WidgetRegistration.Services
{
	public interface ICdnFileSaver
	{
		Task<string> SaveFile(string path, Stream data);
	}

	public class CdnFileSaver : ICdnFileSaver
	{
		private readonly IAmazonS3 _s3Client;
		private readonly ILogger _logger;
		private readonly IConfiguration _configuration;
		private IConfigurationSection awsSettings => _configuration.GetSection("AWSSettings");
		private string BucketName => awsSettings["cdnBucketName"];

		public CdnFileSaver(IAmazonS3 s3Client, ILogger<CdnFileSaver> logger, IConfiguration configuration)
		{
			_s3Client = s3Client;
			_logger = logger;
			_configuration = configuration;
		}

		public async Task<string> SaveFile(string path, Stream data)
		{
			try
			{
				var ms = new MemoryStream();
				await data.CopyToAsync(ms);
				var request = new PutObjectRequest
				{
					BucketName = BucketName,
					Key = path,
					InputStream = ms
				};
				var response = await _s3Client.PutObjectAsync(request);
				return $"https://d2e925blvqf7i7.cloudfront.net/{path}";
			}
			catch (Exception e)
			{
				_logger.Log(LogLevel.Error, e, "Error saving file to bucket");
				throw;
			}
		}
	}
}
