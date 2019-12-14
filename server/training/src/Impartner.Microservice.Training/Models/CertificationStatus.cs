using Impartner.Microservice.Common.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Impartner.Microservice.Training.Models
{
	public class CertificationStatus : TrainingBase, IValidatableObject
	{
		[ReadOnly(true)]
		public int CertificationVersion { get; set; }
		[Required]
		public ObjectId CertificationId { get; set; }
		[ReadOnly(true)]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		[ReadOnly(true)]
		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
		[ReadOnly(true)]
		public DateTime? CompletedAt { get; set; }
		[ReadOnly(true)]
		public User CreatedBy { get; set; }
		[Required]
		[ReadOnly(true)]
		public string Language { get; set; }
		[MaxLength(300)]
		[ReadOnly(true)]
		public string Description { get; set; }
		[Required]
		[Range(0, 36)]
		[ReadOnly(true)]
		public int Expiration { get; set; }
		[Required]
		public List<CourseStatus> Courses { get; set; }
		[ReadOnly(true)]
		public bool IsComplete { get; set; }

		public CertificationStatus() { }

		public CertificationStatus(Certification cert, IEnumerable<Course> courses, User user)
		{
			TenantId = cert.TenantId;
			CertificationVersion = cert.CertificationVersion;
			CreatedBy = user;
			CertificationId = cert.Id;
			Name = cert.Name;
			Description = cert.Description;
			Language = cert.Language;
			Expiration = cert.Expiration;
			IsActive = true;
			Courses = courses.Where(x => x.IsActive).Select(x => new CourseStatus(x, user)).ToList();
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
