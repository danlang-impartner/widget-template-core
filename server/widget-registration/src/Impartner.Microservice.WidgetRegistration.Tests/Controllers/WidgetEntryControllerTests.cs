using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Impartner.Microservice.Common.Exceptions.Http;
using Impartner.Microservice.WidgetRegistration.Controllers;
using Impartner.Microservice.WidgetRegistration.Models;
using Impartner.Microservice.WidgetRegistration.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using MongoDB.Bson;
using Moq;
using Neleus.LambdaCompare;
using Newtonsoft.Json;
using NUnit.Framework;
using WidgetEntry = Impartner.Microservice.WidgetRegistration.Models.WidgetEntry.V1;

namespace Impartner.Microservice.WidgetRegistration.Tests.Controllers
{
	[TestFixture]
	public class WidgetEntryControllerTests : ContextBaseTests
	{
		private WidgetEntryController _controller;
		private Mock<ICdnFileSaver> _fileSaver;
		private Mock<IManifestValidator> _validator;
		private const string CollectionName = WidgetEntryController.CollectionName;

		[SetUp]
		public void SetUp()
		{
			_fileSaver = new Mock<ICdnFileSaver>();
			_validator = new Mock<IManifestValidator>();
			_controller = new WidgetEntryController(Repository.Object, _fileSaver.Object, _validator.Object);
			StubRequest(_controller);
		}

		#region AddNewItem

		[Test]
		public void should_throw_exception_when_no_file_is_uploaded()
		{
			_controller.Awaiting(x => x.AddNewItem(new FormCollection(new Dictionary<string, StringValues>()))).Should()
				.Throw<HttpBadRequestException>().Which.Errors
				.Any(x => x.Message == WidgetEntryController.InvalidPayloadMessage).Should().BeTrue();
		}

		[Test]
		public void should_throw_exception_when_no_manifest_file_located_inside_uploaded_file()
		{
			var file = new FileDefinition {Filename = "test", FileData = Encoding.ASCII.GetBytes("test")};
			var files = TestHelpers.CreateZipFile(new List<FileDefinition> {file});
			var formCollection = new FormCollection(new Dictionary<string, StringValues>(), files);

			_controller.Awaiting(x => x.AddNewItem(formCollection)).Should()
				.Throw<HttpBadRequestException>().Which.Errors
				.Any(x => x.Message == "Unable to locate manifest file").Should().BeTrue();
		}

		[Test]
		public void should_throw_exception_when_validator_returns_invalid_response()
		{
			var manifest = TestHelpers.CreateManifest();
			var json = JsonConvert.SerializeObject(manifest);
			var file = new FileDefinition {Filename = "manifest.json", FileData = Encoding.ASCII.GetBytes(json)};
			var files = TestHelpers.CreateZipFile(new List<FileDefinition> {file});
			var formCollection = new FormCollection(new Dictionary<string, StringValues>(), files);
			StubInvalidManifest();


			_controller.Awaiting(x => x.AddNewItem(formCollection)).Should()
				.Throw<HttpBadRequestException>().Which.Errors
				.Any(x => x.Message == WidgetEntryController.InvalidPayloadMessage).Should().BeTrue();
		}

		private void StubValidManifest()
		{
			_validator.Setup(x => x.Validate(It.IsAny<Manifest>(), It.IsAny<ZipArchive>()))
				.Returns(new ValidationResult());
		}

		private void StubInvalidManifest()
		{
			_validator.Setup(x => x.Validate(It.IsAny<Manifest>(), It.IsAny<ZipArchive>()))
				.Returns(new ValidationResult(new Dictionary<string, List<string>>()));
		}

		[Test]
		public async Task should_save_each_component_from_manifest_file_when_all_files_exist()
		{
			var manifest = TestHelpers.CreateManifest();
			var files = TestHelpers.CreateZipFile(TestHelpers.CreateFilesFromManifest(manifest));
			var formCollection = new FormCollection(new Dictionary<string, StringValues>(), files);
			var widgetCursor = StubCursor<WidgetEntry>(WidgetEntryController.CollectionName);
			StubNotFoundResult(widgetCursor);
			StubValidManifest();

			await _controller.AddNewItem(formCollection);

			_fileSaver.Verify(x => x.SaveFile(It.IsAny<string>(), It.IsAny<Stream>()),
				Times.Exactly(manifest.Components.Count));
		}



		[Test]
		public async Task should_not_find_a_widget_entry_with_name_and_type_saving_a_new_entry()
		{
			var manifest = TestHelpers.CreateManifest();
			var files = TestHelpers.CreateZipFile(TestHelpers.CreateFilesFromManifest(manifest));
			var formCollection = new FormCollection(new Dictionary<string, StringValues>(), files);
			var widgetCursor = StubCursor<WidgetEntry>(WidgetEntryController.CollectionName);
			StubNotFoundResult(widgetCursor);
			StubValidManifest();

			await _controller.AddNewItem(formCollection);

			Repository.Verify(x => x.SaveAsync(WidgetEntryController.CollectionName,
				It.Is<WidgetEntry>(e => AssertEntryIsCreatedCorrectly(e, manifest, 1))));
		}

		private static bool AssertEntryIsCreatedCorrectly(WidgetEntry widgetEntry, Manifest manifest,
			int expectedNumberOfVersions)
		{
			widgetEntry.Versions.Should().HaveCount(expectedNumberOfVersions);
			AssertNewEntryIsCorrect(widgetEntry, manifest);
			return true;
		}

		private static void AssertNewEntryIsCorrect(WidgetEntry widgetEntry, Manifest manifest)
		{
			var entryVersion = widgetEntry.Versions[manifest.Version];
			entryVersion.WidgetComponents.Should().HaveCount(manifest.Components.Count);
			entryVersion.MainIconName.Should().Be(manifest.MainIconName);
			entryVersion.WidgetIcons.Should().BeEquivalentTo(manifest.Icons);
			widgetEntry.Name.Should().Be(manifest.Name);
			widgetEntry.Type.Should().Be(manifest.Type);
			widgetEntry.LatestVersion.Should().Be(manifest.Version);
		}

		[Test]
		public async Task should_find_a_widget_entry_with_name_and_type_saving_a_new_entry_version()
		{
			var manifest = TestHelpers.CreateManifest();
			var files = TestHelpers.CreateZipFile(TestHelpers.CreateFilesFromManifest(manifest));
			var formCollection = new FormCollection(new Dictionary<string, StringValues>(), files);
			var widgetCursor = StubCursor<WidgetEntry>(WidgetEntryController.CollectionName);
			var existingEntry = new WidgetEntry
			{
				Id = ObjectId.GenerateNewId(),
				Name = manifest.Name, Type = manifest.Type,
				Versions = new Dictionary<string, WidgetVersion> {{"0.1", new WidgetVersion()}}
			};
			StubSingleFoundResult(widgetCursor, existingEntry);
			StubValidManifest();

			await _controller.AddNewItem(formCollection);

			Repository.Verify(x => x.UpdateAsync(WidgetEntryController.CollectionName,
				It.IsAny<Expression<Func<WidgetEntry, bool>>>(),
				It.Is<WidgetEntry>(e => AssertEntryIsCreatedCorrectly(e, manifest, 2))));
		}

		[Test]
		public void should_throw_HttpConflictException_when_an_existing_version_already_exists_for_the_specified_manifest()
		{
			var manifest = TestHelpers.CreateManifest();
			var files = TestHelpers.CreateZipFile(TestHelpers.CreateFilesFromManifest(manifest));
			var formCollection = new FormCollection(new Dictionary<string, StringValues>(), files);
			var widgetCursor = StubCursor<WidgetEntry>(WidgetEntryController.CollectionName);
			var existingEntry = new WidgetEntry
			{
				Name = manifest.Name, Type = manifest.Type,
				Versions = new Dictionary<string, WidgetVersion> {{manifest.Version, new WidgetVersion()}}
			};
			StubSingleFoundResult(widgetCursor, existingEntry);
			StubValidManifest();

			_controller.Awaiting(x => x.AddNewItem(formCollection)).Should()
				.Throw<HttpConflictException>().Which.Errors
				.Any(x => x.Message.Contains($"Version {manifest.Version} already exists.")).Should().BeTrue();
		}

		#endregion

		#region GetWidgets

		[Test]
		public async Task should_delegate_to_repo_to_find_filtered_widget_entries()
		{
			var entries = new List<WidgetEntry>();
			var cursor = StubCursor<WidgetEntry>(CollectionName);
			StubListOfResult(cursor, entries);

			await _controller.GetWidgets();

			Repository.Verify(x => x.FindAsync(CollectionName,
				It.Is<Expression<Func<WidgetEntry, bool>>>(e => Lambda.Eq(e, f => f.IsActive))));
		}

		[Test]
		public async Task should_return_only_active_version_of_each_widget_entry()
		{
			const string latestVersion = "1.2";
			const string oldVersion = "1.1";
			var currentVersion = new WidgetVersion();
			var entries = new List<WidgetEntry>
			{
				new WidgetEntry
				{
					LatestVersion = latestVersion,
					Versions = new Dictionary<string, WidgetVersion>
					{
						{oldVersion, new WidgetVersion()},
						{latestVersion, currentVersion},
					}
				},
				new WidgetEntry
				{
					LatestVersion = latestVersion,
					Versions = new Dictionary<string, WidgetVersion>
					{
						{oldVersion, new WidgetVersion()},
						{latestVersion, currentVersion}
					}
				}
			};
			var cursor = StubCursor<WidgetEntry>(CollectionName);
			StubListOfResult(cursor, entries);

			var results = await _controller.GetWidgets();

			results.Data.Count.Should().Be(entries.Count);
		}

		#endregion
	}


	internal class FileDefinition
	{
		public string Filename { get; set; }
		public byte[] FileData { get; set; }
	}
}
