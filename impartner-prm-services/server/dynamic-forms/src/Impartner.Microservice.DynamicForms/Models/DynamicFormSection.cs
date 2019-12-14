using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Impartner.Microservice.DynamicForms.Models
{
	public class DynamicFormSection
	{
		[ReadOnly(true)]
		public ObjectId Id { get; set; }
		[JsonConverter(typeof(StringEnumConverter)), Required, ReadOnly(true)]
		public SectionType Type { get; set; }
		[Required]
		public ItemState State { get; set; }
		[Required]
		public int Size { get; set; } = 12;
		public bool ShowHeader { get; set; } = false;
		public SegmentationObject Segmentation { get; set; } = new SegmentationObject();

		[Required]
		public List<Rule> Rules { get; set; } = new List<Rule>();

		[Required]
		public List<Field> Fields { get; set; } = new List<Field>();
		public Spacing Spacing { get; set; } = new Spacing();

		public DynamicFormSection()
		{
			Id = ObjectId.GenerateNewId();
		}
	}
}
