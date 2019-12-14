using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Impartner.Microservice.DynamicForms.Models
{
	/// <summary>
	/// TODO - Moved to Impartner.Common. Once the project is packaged and importable, it should be used instead.
	/// TODO - <see cref="Impartner.Common.Segmentation.SegmentationRules"/>
	/// </summary>
	public class SegmentationObject
	{
		[Required]
		public List<int> UserFilterIds { get; set; } = new List<int>();
		
		[Required]
		public List<int> AccountFilterIds { get; set; } = new List<int>();
		
		[Required]
		public List<ObjectSegment> UserFieldOptionIds { get; set; } = new List<ObjectSegment>();
		
		[Required]
		public List<ObjectSegment> AccountFieldOptionIds { get; set; } = new List<ObjectSegment>();
		
		[Required]
		public List<int> PartnerLevelIds { get; set; } = new List<int>();
		
		[Required]
		public List<int> TierIds { get; set; } = new List<int>();
		
		[Required]
		public List<int> RegionIds { get; set; } = new List<int>();
	}
}
