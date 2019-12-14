using System;
using System.Collections.Generic;
using Impartner.Microservice.Common.Models;

namespace Impartner.Microservice.WidgetRegistration.Models
{
	public class WidgetVersion
	{
		public User CreatedBy { get; set; }
		public DateTime CreatedAt { get; set; }
		public string MainIconName { get; set; }
		public List<WidgetIcon> WidgetIcons { get; set;  } = new List<WidgetIcon>();
		public List<WidgetComponent> WidgetComponents { get; set;  } = new List<WidgetComponent>();
	}
}
