using Impartner.Microservice.Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Impartner.Microservice.Training.Models
{
	public class Certification : TrainingBase, IValidatableObject
	{
		[ReadOnly(true)]
		public int CertificationVersion { get; set; } = 0;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
		public User CreatedBy { get; set; }
		public User UpdatedBy { get; set; }
		[Required]
		public string Language { get; set; }
		[MaxLength(300)]
		public string Description { get; set; }
		[Range(0, 36)]
		public int Expiration { get; set; }
		public SegmentationObject Segmentation { get; set; } = new SegmentationObject();
		public List<DenormalizedCourse> Courses { get; set; } = new List<DenormalizedCourse>();

		public Certification ShallowCopy()
		{
			return MemberwiseClone() as Certification;
		}

		/// <summary>Determines whether the specified object is valid.</summary>
		/// <param name="validationContext">The validation context.</param>
		/// <returns>A collection that holds failed-validation information.</returns>
		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if (Expiration % 12 != 0)
			{
				yield return new ValidationResult($"{nameof(Expiration)} value must be a multiple of 12");
			}
		}
	}
}
