using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Impartner.Microservice.WidgetRegistration.Models;
using Impartner.Microservice.WidgetRegistration.Services;
using Impartner.Microservice.WidgetRegistration.Tests.Controllers;
using NUnit.Framework;

namespace Impartner.Microservice.WidgetRegistration.Tests.Services
{
	[TestFixture]
	public class ManifestValidatorTests
	{
		private ManifestValidator _validator;
		private Manifest _manifest;

		[SetUp]
		public void SetUp()
		{
			_manifest = TestHelpers.CreateManifest();
			_validator = new ManifestValidator();
		}

		#region Validate

		[Test]
		public void should_return_result_with_valid_result()
		{
			var files = TestHelpers.CreateFilesFromManifest(_manifest);
			var outStream = new MemoryStream();
			var zip = TestHelpers.CreateZipArchive(outStream, files);

			var result = _validator.Validate(_manifest, zip);

			result.IsValid.Should().BeTrue();
		}

		[Test]
		public void should_return_result_with_invalid_result_when_files_are_missing()
		{
			var files = TestHelpers.CreateFilesFromManifest(_manifest);
			var outStream = new MemoryStream();
			var zip = TestHelpers.CreateZipArchive(outStream, new List<FileDefinition>{files[0]});

			var result = _validator.Validate(_manifest, zip);

			result.IsValid.Should().BeFalse();
			result.ErrorDetails.Should().ContainKey(ManifestValidator.MissingFiles);
		}

		[Test]
		public void should_return_result_with_invalid_result_when_type_does_not_start_with_custom()
		{
			_manifest.Type = "bad.format";
			var files = TestHelpers.CreateFilesFromManifest(_manifest);
			var outStream = new MemoryStream();
			var zip = TestHelpers.CreateZipArchive(outStream, files);

			var result = _validator.Validate(_manifest, zip);

			result.IsValid.Should().BeFalse();
			result.ErrorDetails.Should().ContainKey(ManifestValidator.MustStartWithCustom);
		}

		[Test]
		public void should_return_result_with_invalid_result_when_there_are_any_duplicate_widget_modes()
		{
			_manifest.Components.ForEach(c => c.WidgetMode = WidgetMode.View);
			var files = TestHelpers.CreateFilesFromManifest(_manifest);
			var outStream = new MemoryStream();
			var zip = TestHelpers.CreateZipArchive(outStream, files);

			var result = _validator.Validate(_manifest, zip);

			result.IsValid.Should().BeFalse();
			result.ErrorDetails.Should().ContainKey(ManifestValidator.DuplicateWidgetMode);
		}

		#endregion
	}
}
