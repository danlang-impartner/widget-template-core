using Impartner.Microservice.Training.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Impartner.Microservice.Training.Services
{
	public interface ICertificationStatusUpdator
	{
		CertificationStatus Update(CertificationStatus certStatus);
	}

	public class CertificationStatusUpdator : ICertificationStatusUpdator
	{
		public const string NoAnswerErrorFormat = "No answer was provided for question: \"{0}\"";

		public CertificationStatus Update(CertificationStatus certStatus)
		{
			certStatus.UpdatedAt = DateTime.UtcNow;
			foreach (var courseStatus in certStatus.Courses)
			{
				courseStatus.Quizzes.ForEach(ValidateQuizzes);
				if (!AllQuizzesAreCompleted(courseStatus))
					return certStatus;

				courseStatus.CompletedAt = DateTime.UtcNow;
				courseStatus.HasPassed = AllQuizzesArePassed(courseStatus);
				if (courseStatus.HasPassed)
					courseStatus.IsLocked = true;
			}
			return certStatus;
		}

		private static bool AllQuizzesArePassed(CourseStatus courseStatus)
		{
			return courseStatus.Quizzes.All(q => q.QuizStatuses.Any(s => s.HasPassed));
		}

		private static bool AllQuizzesAreCompleted(CourseStatus courseStatus)
		{
			var quizStatuses = new List<QuizState>();
			courseStatus.Quizzes.ForEach(x => quizStatuses.AddRange(x.QuizStatuses));
			return quizStatuses.All(x => x.IsComplete);
		}

		private static void ValidateQuizzes(CertStatusQuiz quiz)
		{
			foreach (var quizState in quiz.QuizStatuses)
			{
				if (quizState.Status != QuizStatus.Completed) continue;
				quizState.IsComplete = true;
				quizState.CorrectQuestions = quiz.Questions.Sum(q => ValidateAnswer(q, quizState.Answers));
				quizState.HasPassed = quizState.CorrectQuestions >= quiz.MinimumCorrectAnswers;
			}
		}

		private static int ValidateAnswer(CertStatusQuestion question, IEnumerable<QuizAnswer> answers)
		{
			var answer = answers.FirstOrDefault(x => x.QuestionId == question.QuestionId.ToString());
			if (answer == null)
				throw new InvalidOperationException(string.Format(NoAnswerErrorFormat, question.Statement));

			answer.IsCorrect = answer.SelectedAnswers.OrderBy(x => x)
				.SequenceEqual(question.CorrectAnswers.OrderBy(x => x));
			return answer.IsCorrect ? 1 : 0;
		}
	}
}
