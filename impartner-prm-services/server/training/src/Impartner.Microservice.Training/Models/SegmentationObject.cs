using System.Collections.Generic;

namespace Impartner.Microservice.Training.Models
{
	/// <summary>
	/// TODO - Moved to Impartner.Common. Once the project is packaged and importable, it should be used instead.
	/// TODO - <see cref="Impartner.Common.Segmentation.SegmentationRules"/>
	/// </summary>
	public class SegmentationObject
	{
		public List<int> UserFilterIds { get; set; } = new List<int>();
		public List<int> AccountFilterIds { get; set; } = new List<int>();
		public List<ObjectSegment> UserFieldOptionIds { get; set; } = new List<ObjectSegment>();
		public List<ObjectSegment> AccountFieldOptionIds { get; set; } = new List<ObjectSegment>();
		public List<int> PartnerLevelIds { get; set; } = new List<int>();
		public List<int> TierIds { get; set; } = new List<int>();
		public List<int> RegionIds { get; set; } = new List<int>();
	}
}
