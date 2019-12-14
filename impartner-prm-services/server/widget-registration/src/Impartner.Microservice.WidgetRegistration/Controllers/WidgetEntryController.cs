using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Impartner.Microservice.Common.Attributes;
using Impartner.Microservice.Common.Authorization;
using Impartner.Microservice.Common.Exceptions.Http;
using Impartner.Microservice.Common.Extensions;
using Impartner.Microservice.Common.Models.Responses;
using Impartner.Microservice.Common.Mongo.Repositories;
using Impartner.Microservice.WidgetRegistration.Extensions;
using Impartner.Microservice.WidgetRegistration.Models;
using Impartner.Microservice.WidgetRegistration.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using WidgetEntry = Impartner.Microservice.WidgetRegistration.Models.WidgetEntry.V1;

namespace Impartner.Microservice.WidgetRegistration.Controllers
{
	[ApiController]
	[Route("api/ms/v1/widget-entry")]
	[Authorize(PolicyNames.TenantId)]
	[Produces("application/json")]
	public class WidgetEntryController : ControllerBase
	{
		public const string CollectionName = "widgetEntry";
		public const string InvalidPayloadMessage = "Widget payload is invalid";
		private readonly IMongoRepository _repository;
		private readonly ICdnFileSaver _cdnFileSaver;
		private readonly IManifestValidator _validator;

		public WidgetEntryController(IMongoRepository repository, ICdnFileSaver cdnFileSaver,
			IManifestValidator validator)
		{
			_repository = repository;
			_cdnFileSaver = cdnFileSaver;
			_validator = validator;
		}


		[ProducesApiResult(HttpStatusCode.BadRequest, description: InvalidPayloadMessage)]
		[ProducesApiResult(HttpStatusCode.Forbidden,
			description: "MongoDB failed to save the new item; Can be resubmitted")]
		[ProducesApiResult(HttpStatusCode.Conflict,
			description: "Version {manifest.Version} already exists.")]
		[ProducesApiResult(HttpStatusCode.OK, typeof(WidgetEntry),
			"Creates a new widget entry based on the values provided")]
		[HttpPost]
		public async Task<ApiResult<WidgetEntry>> AddNewItem([FromForm] IFormCollection files)
		{
			var fileData = GetFileData(files);
			var manifestWidgetVersion = await CreateWidgetVersion(fileData);
			var entry = await FindOrCreateEntry(manifestWidgetVersion.Manifest);
			return await SaveNewVersion(manifestWidgetVersion, entry);
		}

		private static Stream GetFileData(IFormCollection fileCollection)
		{
			if (fileCollection.Files.Count != 1)
			{
				throw new HttpBadRequestException
				(
					new ApiError(
						nameof(BadRequest),
						InvalidPayloadMessage,
						additionalData: new Dictionary<string, string>
						{
							{"missingPayload", "A payload file is required."}
						})
				);
			}

			var file = fileCollection.Files.First();
			return file.OpenReadStream();
		}

		private async Task<ManifestWidgetVersion> CreateWidgetVersion(Stream fileData)
		{
			using (var archive = new ZipArchive(fileData, ZipArchiveMode.Read))
			{
				var manifest = FindManifest(archive);
				var components = await CreateWidgetComponents(manifest, archive);
				return new ManifestWidgetVersion
				{
					WidgetVersion = new WidgetVersion
					{
						MainIconName = manifest.MainIconName,
						WidgetComponents = components,
						WidgetIcons = manifest.Icons
					},
					Manifest = manifest
				};
			}
		}

		private Manifest FindManifest(ZipArchive archive)
		{
			const string manifestFileName = "manifest.json";
			var manifestJson = archive.GetEntry(manifestFileName);
			if (manifestJson == null)
			{
				throw new HttpBadRequestException
				(
					new ApiError(
						nameof(BadRequest),
						"Unable to locate manifest file",
						additionalData: new Dictionary<string, string> {{"manifest", manifestFileName}})
				);
			}

			var manifest = JsonConvert.DeserializeObject<Manifest>(manifestJson.EntryToString());
			ValidateManifest(manifest, archive);
			return manifest;
		}

		private void ValidateManifest(Manifest manifest, ZipArchive archive)
		{
			var results = _validator.Validate(manifest, archive);
			if (!results.IsValid)
			{
				throw new HttpBadRequestException
				(
					new ApiError(
						nameof(BadRequest),
						InvalidPayloadMessage,
						additionalData: results.ErrorDetails)
				);
			}
		}

		private async Task<List<WidgetComponent>> CreateWidgetComponents(Manifest manifest, ZipArchive archive)
		{
			var tenant = User.ToTenantId();
			var basePath = $"{tenant}/{manifest.Type}/{Guid.NewGuid()}";
			var components = new List<WidgetComponent>();
			foreach (var component in manifest.Components)
			{
				var widgetComponent = await SaveComponent(component, archive, basePath);
				components.Add(widgetComponent);
			}

			return components;
		}

		private async Task<WidgetComponent> SaveComponent(Component component, ZipArchive archive, string basePath)
		{
			var file = archive.GetEntry(component.Source);
			if (file == null)
			{
				throw new MissingMemberException();
			}

			var url = await _cdnFileSaver.SaveFile($"{basePath}/{file.Name}", file.Open());
			return new WidgetComponent
			{
				WidgetMode = component.WidgetMode,
				TagId = component.TagId,
				SourceUrl = url
			};
		}

		private async Task<WidgetEntry> FindOrCreateEntry(Manifest manifest)
		{
			var results = await _repository.FindAsync<WidgetEntry>(CollectionName,
				x => x.Type == manifest.Type && x.Name == manifest.Name);
			var entry = results.SingleOrDefault();
			if (entry == null)
			{
				return new WidgetEntry
				{
					Name = manifest.Name,
					Type = manifest.Type,
					IsActive = true,
					CreatedAt = DateTime.UtcNow,
					CreatedBy = User.ToUserInfo()
				};
			}

			if (entry.Versions.ContainsKey(manifest.Version))
			{
				throw new HttpConflictException(
					new ApiError
					(
						nameof(Conflict),
						$"Version {manifest.Version} already exists."
					)
				);
			}

			return entry;
		}

		private async Task<ApiResult<WidgetEntry>> SaveNewVersion(ManifestWidgetVersion manifestWidgetVersion,
			WidgetEntry entry)
		{
			entry.AddVersion(manifestWidgetVersion, User.ToUserInfo());
			if (entry.Id == ObjectId.Empty)
				return await _repository.SaveAsync(CollectionName, entry);
			var result = await _repository.UpdateAsync(CollectionName, x => x.Id == entry.Id, entry);
			return entry;
		}

		[HttpGet]
		[ProducesApiResult(HttpStatusCode.OK, typeof(List<WidgetVersionOutput>), "Gets all registered widgets.")]
		public async Task<ApiResult<IList<WidgetVersionOutput>>> GetWidgets()
		{
			var asyncCursor = await _repository.FindAsync<WidgetEntry>(CollectionName, x => x.IsActive);
			var entries = asyncCursor.ToList();
			return entries.ToWidgetVersionOutput();
		}
	}
}
