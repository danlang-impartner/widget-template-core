using MongoDB.Bson;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Impartner.Microservice.Training.Models
{
	public class Quiz : TrainingBase
	{
		[Required]
		public int MinimumCorrectAnswers { get; set; }
		[Required]
		public List<Question> Questions { get; set; } = new List<Question>();
		public bool RandomizeQuestions { get; set; }
		public bool RandomizeAnswers { get; set; }
	}

	public class CertStatusQuiz : TrainingBase
	{
		[ReadOnly(true)]
		public ObjectId QuizId { get; set; }
		[ReadOnly(true)]
		public int MinimumCorrectAnswers { get; set; }
		[ReadOnly(true)]
		public List<CertStatusQuestion> Questions { get; set; }
		public List<QuizState> QuizStatuses { get; set; } = new List<QuizState>();
		public bool RandomizeQuestions { get; set; }
		public bool RandomizeAnswers { get; set; }

		public CertStatusQuiz() { }

		public CertStatusQuiz(Quiz quiz)
		{
			QuizId = quiz.Id;
			Name = quiz.Name;
			MinimumCorrectAnswers = quiz.MinimumCorrectAnswers;
			Questions = quiz.Questions.Select(x => new CertStatusQuestion(x)).ToList();
			QuizStatuses = new List<QuizState> { new QuizState() };
			IsActive = true;
			RandomizeQuestions = quiz.RandomizeQuestions;
			RandomizeAnswers = quiz.RandomizeAnswers;
		}
	}
}
