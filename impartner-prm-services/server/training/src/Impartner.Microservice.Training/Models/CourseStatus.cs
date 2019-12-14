using Impartner.Microservice.Common.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Impartner.Microservice.Training.Models
{
	public class CourseStatus : TrainingBase
	{
		[Required]
		public int CourseVersion;
		[Required]
		public ObjectId CertificationId;
		[Required]
		public ObjectId CourseId;
		[ReadOnly(true)]
		[MaxLength(300)]
		public string Description { get; set; }
		[ReadOnly(true)]
		public List<CertStatusLesson> Lessons { get; set; }
		[ReadOnly(true)]
		public List<CertStatusQuiz> Quizzes { get; set; }
		[ReadOnly(true)]
		public DateTime? CompletedAt { get; set; }
		[ReadOnly(true)]
		public User CreatedBy { get; set; }
		[ReadOnly(true), BsonIgnore]
		public int LessonsCompleted => Lessons.Count(x => x.IsCompleted);
		[JsonIgnore]
		public bool HasPassed { get; set; }
		[JsonIgnore]
		public bool IsLocked { get; set; }

		public CourseStatus() { }

		public CourseStatus(Course course, User user)
		{
			TenantId = course.TenantId;
			CourseVersion = course.CourseVersion;
			CertificationId = ObjectId.Parse(course.CertificationId);
			CourseId = course.Id;
			Name = course.Name;
			Description = course.Description;
			Lessons = course.Lessons.Where(x => x.IsActive).Select(x => new CertStatusLesson(x)).ToList();
			Quizzes = course.Quizzes.Where(x => x.IsActive).Select(x => new CertStatusQuiz(x)).ToList();
			CreatedBy = user;
			IsActive = true;
		}

		public void RestartQuiz(CertStatusQuiz quiz)
		{
			quiz.QuizStatuses.Where(x => !x.IsComplete).ToList().ForEach(CompleteQuiz);
			quiz.QuizStatuses.Add(new QuizState());
		}

		private static void CompleteQuiz(QuizState quiz)
		{
			quiz.HasPassed = false;
			quiz.IsComplete = true;
			quiz.Status = QuizStatus.Restarted;
		}
	}
}
