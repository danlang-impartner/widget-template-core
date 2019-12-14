using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Impartner.Microservice.DynamicForms.Models
{
	public class Field
	{
		[Required, ReadOnly(true)]
		public string Id { get; set; }
		[Required]
		public ItemState State { get; set; }
		[Required]
		public int? Size { get; set; }
		public SegmentationObject Segmentation { get; set; } = new SegmentationObject();
		public List<Validation> Validations { get; set; }  = new List<Validation>();
		public List<Rule> Rules { get; set; } = new List<Rule>();
	}
}
