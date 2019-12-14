using System.Collections.Generic;

namespace Impartner.Microservice.WidgetRegistration.Models
{
	public class Manifest
	{
		public string ManifestVersion { get; set; }
		public string Type { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string MainIconName { get; set; }
		public string Version { get; set; }
		public string Author { get; set; }
		public List<WidgetIcon> Icons { get; set; } = new List<WidgetIcon>();
		public List<Component> Components { get; set; } = new List<Component>();
	}
}
