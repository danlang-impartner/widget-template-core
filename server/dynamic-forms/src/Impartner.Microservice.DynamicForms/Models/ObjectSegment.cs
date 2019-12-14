using System.ComponentModel.DataAnnotations;

namespace Impartner.Microservice.DynamicForms.Models
{
	/// <summary>
	/// TODO - Moved to Impartner.Common. Once the project is packaged and importable, it should be used instead.
	/// TODO - <see cref="Impartner.Common.Segmentation.FieldSegmentationRule"/>
	/// </summary>
	public class ObjectSegment
	{
		[Required]
		public int SegmentId { get; set; }
		
		[Required]
		public int OptionId { get; set; }
	}
}
