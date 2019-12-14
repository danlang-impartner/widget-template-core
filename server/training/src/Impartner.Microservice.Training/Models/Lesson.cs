using MongoDB.Bson;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Impartner.Microservice.Training.Models
{
	public class Lesson : TrainingBase
	{
		[Required]
		public LessonContentTypeEnum ContentType { get; set; }
		public LessonContent Content { get; set; }
	}

	public class CertStatusLesson : TrainingBase
	{
		[ReadOnly(true)]
		public ObjectId LessonId { get; set; }
		[ReadOnly(true)]
		public LessonContentTypeEnum ContentType { get; set; }
		[ReadOnly(true)]
		public LessonContent Content { get; set; }
		public bool IsCompleted { get; set; }

		public CertStatusLesson() { }

		public CertStatusLesson(Lesson lesson)
		{
			LessonId = lesson.Id;
			Name = lesson.Name;
			ContentType = lesson.ContentType;
			Content = lesson.Content;
			IsActive = true;
		}
	}
}
