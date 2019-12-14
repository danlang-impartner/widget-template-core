using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using Impartner.Microservice.WidgetRegistration.Models;

namespace Impartner.Microservice.WidgetRegistration.Services
{
	public interface IManifestValidator
	{
		ValidationResult Validate(Manifest manifest, ZipArchive archive);
	}

	public class ManifestValidator : IManifestValidator
	{
		public const string MustStartWithCustom = "The component type must follow the format custom.{vendor}.{name}";
		public const string DuplicateWidgetMode = "There are duplicate widget modes listed.";
		public const string MissingFiles = "There are missing files listed in the manifest.";
		public const string WidgetPrefix = "custom.";

		public ValidationResult Validate(Manifest manifest, ZipArchive archive)
		{
			var errors = new Dictionary<string, List<string>>();
			if (!manifest.Type.StartsWith(WidgetPrefix, StringComparison.OrdinalIgnoreCase))
			{
				errors.Add(MustStartWithCustom, new List<string>{manifest.Type});
			}
			var missingFiles = new List<string>();
			var modes = new HashSet<WidgetMode>();
			var duplicateModes = new List<string>();
			foreach (var component in manifest.Components)
			{
				var entry = archive.GetEntry(component.Source);
				if (entry == null)
					missingFiles.Add(component.Source);
				var wasAdded = modes.Add(component.WidgetMode);
				if (!wasAdded)
				{
					duplicateModes.Add($"Component {component.TagId} is using a duplicate widget mode of {component.WidgetMode}");
				}
			}

			if (duplicateModes.Any())
			{
				errors.Add(DuplicateWidgetMode, duplicateModes);
			}

			if (missingFiles.Any())
			{
				errors.Add(MissingFiles, missingFiles);
			}

			return errors.Keys.Any() ? new ValidationResult(errors) : new ValidationResult();
		}
	}
}
