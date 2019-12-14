using Amazon.S3;
using Amazon.S3.Model;
using FluentAssertions;
using Impartner.Microservice.Training.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Impartner.Microservice.Training.Tests.Services
{
	[TestFixture]
	public class MessengerTests
	{
		private const string ObjectName = "Test";
		private Messenger _messenger;
		private Mock<IAmazonS3> _s3Client;
		private Mock<ILogger<Messenger>> _logger;
		private Mock<IHttpContextAccessor> _contextAccessor;
		private const string EnvironmentName = "Development";
		private const string AwsEnvironmentName = "dev";

		[SetUp]
		public void SetUp()
		{
			_s3Client = new Mock<IAmazonS3>();
			_logger = new Mock<ILogger<Messenger>>();
			_contextAccessor = new Mock<IHttpContextAccessor>();
			var headers = new HeaderDictionary { { "X-Environment-Context", new StringValues(EnvironmentName) } };
			_contextAccessor.Setup(x => x.HttpContext.Request.Headers).Returns(headers);
			_messenger = new Messenger(_s3Client.Object, _logger.Object, _contextAccessor.Object);
		}

		[Test]
		public void should_concat_bucket_root_name_with_environment_name()
		{
			var expectedName = $"{Messenger.BucketRoot}{AwsEnvironmentName}";

			var bucketName = _messenger.BucketName;

			bucketName.Should().Be(expectedName);
		}

		#region SendMessage

		[Test]
		public async Task should_send_put_request_returning_true_when_saved_successful()
		{
			_s3Client.Setup(c => c.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new PutObjectResponse());

			var result = await _messenger.SendMessage(ObjectName, new { });

			_s3Client.Verify(c =>
				c.PutObjectAsync(
					It.Is<PutObjectRequest>(x =>
						x.BucketName == _messenger.BucketName &&
						x.Metadata[Messenger.ObjectNameMetaField] == ObjectName &&
						x.InputStream != null), It.IsAny<CancellationToken>()));
			result.Should().BeTrue();
		}

		[Test]
		public async Task should_return_false_when_it_fails_to_save_successful()
		{
			var exception = new Exception("BOOM!");
			_s3Client.Setup(c => c.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
				.ThrowsAsync(exception);

			var result = await _messenger.SendMessage(ObjectName, new { });

			_s3Client.Verify(c =>
				c.PutObjectAsync(
					It.Is<PutObjectRequest>(x =>
						x.BucketName == _messenger.BucketName &&
						x.Metadata[Messenger.ObjectNameMetaField] == ObjectName &&
						x.InputStream != null), It.IsAny<CancellationToken>()));
			result.Should().BeFalse();
			_logger.Verify(l => l.Log(LogLevel.Error, 0, It.IsAny<object>(), exception, It.IsAny<Func<object, Exception, string>>()));
		}

		#endregion
	}
}
