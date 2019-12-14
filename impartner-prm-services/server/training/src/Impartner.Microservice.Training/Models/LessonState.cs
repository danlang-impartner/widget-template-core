using MongoDB.Bson;

namespace Impartner.Microservice.Training.Models
{
	public class LessonState
	{
		public bool IsCompleted { get; set; }
		public ObjectId LessonId { get; set; }

		public LessonState()
		{
		}

		public LessonState(ObjectId lessonId)
		{
			LessonId = lessonId;
		}
	}
}