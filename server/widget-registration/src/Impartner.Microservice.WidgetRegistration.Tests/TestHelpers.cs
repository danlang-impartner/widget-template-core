using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Impartner.Microservice.WidgetRegistration.Models;
using Impartner.Microservice.WidgetRegistration.Services;
using Impartner.Microservice.WidgetRegistration.Tests.Controllers;
using Microsoft.AspNetCore.Http.Internal;
using Newtonsoft.Json;

namespace Impartner.Microservice.WidgetRegistration.Tests
{
	public static class TestHelpers
	{
		public static Manifest CreateManifest()
		{
			return new Manifest
			{
				Name = "Name",
				Version = "1.1",
				Description = "Description here",
				Type = $"{ManifestValidator.WidgetPrefix}vendor.name",
				Components = new List<Component>
				{
					new Component
					{
						Source = "doesnt exist1", TagId = "test1", WidgetMode = WidgetMode.Edit
					},
					new Component
					{
						Source = "doesnt exist2", TagId = "test2", WidgetMode = WidgetMode.View
					}
				}
			};
		}

		internal static ZipArchive CreateZipArchive(Stream outStream, IEnumerable<FileDefinition> definitions)
		{
			var zipFile = new ZipArchive(outStream, ZipArchiveMode.Update, true);
			using (zipFile)
			{
				foreach (var definition in definitions)
				{
					var fileInArchive = zipFile.CreateEntry(definition.Filename, CompressionLevel.Optimal);
					using (var entryStream = fileInArchive.Open())
					using (var fileToCompressStream = new MemoryStream(definition.FileData))
					{
						fileToCompressStream.CopyTo(entryStream);
					}
				}
			}

			return zipFile;
		}

		internal static FormFileCollection CreateZipFile(IEnumerable<FileDefinition> definitions)
		{
			var outStream = new MemoryStream();
			CreateZipArchive(outStream, definitions);
			return new FormFileCollection
			{
				new FormFile(outStream, 0, outStream.Length, "Data", "dummy.zip")
			};
		}

		internal static IList<FileDefinition> CreateFilesFromManifest(Manifest manifest)
		{
			var json = JsonConvert.SerializeObject(manifest);
			var files = new List<FileDefinition>
			{
				new FileDefinition {Filename = "manifest.json", FileData = Encoding.ASCII.GetBytes(json)}
			};
			files.AddRange(manifest.Components.Select(component => new FileDefinition
			{
				Filename = component.Source, FileData = Encoding.ASCII.GetBytes("js data here")
			}));
			return files;
		}
	}
}
