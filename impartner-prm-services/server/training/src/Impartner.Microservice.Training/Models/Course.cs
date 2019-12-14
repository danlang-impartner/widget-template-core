using Impartner.Microservice.Common.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Impartner.Microservice.Training.Models
{
	public class Course : TrainingBase
	{
		[ReadOnly(true)]
		public int CourseVersion { get; set; } = 0;
		[Required]
		[BsonRepresentation(BsonType.ObjectId)]
		public string CertificationId;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
		[MaxLength(300)]
		public string Description { get; set; }
		public User CreatedBy { get; set; }
		public User UpdatedBy { get; set; }
		public List<Lesson> Lessons { get; set; } = new List<Lesson>();
		public List<Quiz> Quizzes { get; set; } = new List<Quiz>();
		[BsonIgnore]
		public int ActiveLessonCount => Lessons.Count(l => l.IsActive);
		[BsonIgnore]
		public int ActiveQuizCount => Quizzes.Count(q => q.IsActive);

		public Course ShallowCopy()
		{
			return MemberwiseClone() as Course;
		}
	}
}
