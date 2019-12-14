using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using FluentAssertions;
using Impartner.Microservice.WidgetRegistration.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Impartner.Microservice.WidgetRegistration.Tests.Services
{
	[TestFixture]
	public class CdnFileSaverTests
	{
		private const string ObjectName = "Test";
		private CdnFileSaver _cdnFileSaver;
		private Mock<IAmazonS3> _s3Client;
		private Mock<ILogger<CdnFileSaver>> _logger;
		private Mock<IConfiguration> _config;
		private Mock<IConfigurationSection> _section;
		private Stream _stream;
		private const string _path = "Some path here";
		private const string Bucketname = "BucketName";

		[SetUp]
		public void SetUp()
		{
			_s3Client = new Mock<IAmazonS3>();
			_logger = new Mock<ILogger<CdnFileSaver>>();
			_config = new Mock<IConfiguration>();
			_cdnFileSaver = new CdnFileSaver(_s3Client.Object, _logger.Object, _config.Object);

			_stream = new MemoryStream();
			_section = new Mock<IConfigurationSection>();
			_config.Setup(x => x.GetSection(It.IsAny<string>())).Returns(_section.Object);
			_section.Setup(x => x[It.IsAny<string>()]).Returns(Bucketname);
		}

		#region SaveFile

		[Test]
		public async Task should_send_put_request_returning_full_url_when_saved_successful()
		{
			_s3Client.Setup(c => c.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new PutObjectResponse());

			var result = await _cdnFileSaver.SaveFile(_path, _stream);

			_s3Client.Verify(c =>
				c.PutObjectAsync(
					It.Is<PutObjectRequest>(x =>
						x.BucketName == Bucketname && x.InputStream != null), It.IsAny<CancellationToken>()));
			result.Should().Contain(_path);
		}

		#endregion
	}
}
