using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Impartner.Microservice.WidgetRegistration.Models;
using WidgetEntry = Impartner.Microservice.WidgetRegistration.Models.WidgetEntry.V1;

namespace Impartner.Microservice.WidgetRegistration.Extensions
{
	public static class ExtensionMethods
	{
		public static string EntryToString(this ZipArchiveEntry entry)
		{
			var s = entry.Open();
			var sr = new StreamReader(s);
			return sr.ReadToEnd();
		}

		public static List<WidgetVersionOutput> ToWidgetVersionOutput(this IEnumerable<WidgetEntry> entries)
		{
			return entries.Select(e => e.Versions[e.LatestVersion].ToWidgetVersionOutput(e)).ToList();
		}

		private static WidgetVersionOutput ToWidgetVersionOutput(this WidgetVersion version, WidgetEntry entry)
		{
			return new WidgetVersionOutput
			{
				CreatedAt = version.CreatedAt,
				CreatedBy = version.CreatedBy,
				WidgetComponents = version.WidgetComponents,
				WidgetIcons = version.WidgetIcons,
				MainIconName = version.MainIconName,
				Type = entry.Type,
				Name = entry.Name
			};
		}
	}
}
