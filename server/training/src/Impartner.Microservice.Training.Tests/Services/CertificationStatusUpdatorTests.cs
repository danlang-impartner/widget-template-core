using FluentAssertions;
using Impartner.Microservice.Common.Models;
using Impartner.Microservice.Training.Models;
using Impartner.Microservice.Training.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Impartner.Microservice.Training.Tests.Services
{
	[TestFixture]
	public class CertificationStatusUpdatorTests
	{
		private CertificationStatusUpdator _updator;
		private CertificationStatus _status;
		private Question _question;
		private DateTime _now;
		private const string ObjectIdString = "5cf990d822ccb41e08181429";

		[SetUp]
		public void SetUp()
		{
			_updator = new CertificationStatusUpdator();
			_now = DateTime.UtcNow;
			_status = new CertificationStatus(new Certification(), CreateCourses(), new User());
		}

		#region Update

		[Test]
		public void should_set_cert_status_updated_time_to_be_now_but_not_mark_status_as_completed_if_not_all_courses_are_passed()
		{
			var result = _updator.Update(_status);

			result.UpdatedAt.Should().BeCloseTo(_now, 200);
			result.Courses.First().IsLocked.Should().BeFalse();
			result.Courses.First().CompletedAt.Should().BeNull();
		}

		[Test]
		public void should_throw_exception_when_there_is_a_quiz_state_missing_an_answer()
		{
			var quiz = _status.Courses.First().Quizzes.First();
			var quizState = quiz.QuizStatuses.First();
			quizState.Status = QuizStatus.Completed;
			var error = string.Format(CertificationStatusUpdator.NoAnswerErrorFormat, quiz.Questions.First().Statement);

			_updator.Invoking(x => x.Update(_status))
				.Should().Throw<InvalidOperationException>()
				.WithMessage(error);
		}

		[Test]
		public void should_set_answer_to_be_incorrect_when_incorrect_answer_given()
		{
			var quizState = _status.Courses.First().Quizzes.First().QuizStatuses.First();
			quizState.Status = QuizStatus.Completed;
			quizState.Answers.Add(new QuizAnswer { QuestionId = _question.Id.ToString(), SelectedAnswers = new List<string> { "Nope" } });

			var result = _updator.Update(_status);

			result.UpdatedAt.Should().BeCloseTo(_now, 300);
			var courseStatus = result.Courses.First();
			courseStatus.HasPassed.Should().BeFalse();
			courseStatus.IsLocked.Should().BeFalse();
			var resultQuiz = courseStatus.Quizzes.First().QuizStatuses.First();
			resultQuiz.IsComplete.Should().BeTrue();
			resultQuiz.HasPassed.Should().BeFalse();
			resultQuiz.CorrectQuestions.Should().Be(0);
			resultQuiz.Answers[0].IsCorrect.Should().BeFalse();
		}

		[Test]
		public void should_set_answer_to_be_correct_when_correct_answer_given()
		{
			var quizState = _status.Courses.First().Quizzes.First().QuizStatuses.First();
			quizState.Status = QuizStatus.Completed;
			quizState.Answers.Add(new QuizAnswer { QuestionId = _question.Id.ToString(), SelectedAnswers = new List<string> { "Ok", "Yep" } });

			var result = _updator.Update(_status);

			result.UpdatedAt.Should().BeCloseTo(_now, 300);
			var courseStatus = result.Courses.First();
			courseStatus.CompletedAt.Should().BeCloseTo(_now, 300);
			courseStatus.HasPassed.Should().BeTrue();
			courseStatus.IsLocked.Should().BeTrue();
			var resultQuiz = courseStatus.Quizzes.First().QuizStatuses.First();
			resultQuiz.IsComplete.Should().BeTrue();
			resultQuiz.HasPassed.Should().BeTrue();
			resultQuiz.CorrectQuestions.Should().Be(1);
			resultQuiz.Answers[0].IsCorrect.Should().BeTrue();
		}

		#endregion

		#region AllQuizzesArePassed

		[Test]
		public void should_set_has_passed_to_true_if_each_quiz_has_one_successful_quiz_state()
		{
			var failedQuizState = _status.Courses.First().Quizzes.First().QuizStatuses.First();
			failedQuizState.Status = QuizStatus.Completed;
			failedQuizState.Answers.Add(new QuizAnswer { QuestionId = _question.Id.ToString(), SelectedAnswers = new List<string> { "Nope" } });
			var successfulQuizState = new QuizState { HasPassed = true, IsComplete = true, Status = QuizStatus.Completed, Answers = new List<QuizAnswer>() };
			successfulQuizState.Answers.Add(new QuizAnswer { QuestionId = _question.Id.ToString(), SelectedAnswers = new List<string> { "Ok", "Yep" } });
			_status.Courses.First().Quizzes.First().QuizStatuses.Add(successfulQuizState);

			var result = _updator.Update(_status);

			result.Courses.First().HasPassed.Should().BeTrue();
		}

		#endregion

		#region Helper methods

		private IEnumerable<Course> CreateCourses()
		{
			return new List<Course>
			{
				new Course
				{
					IsActive = true,
					CertificationId = ObjectIdString,
					CreatedBy = new User(),
					Quizzes = CreateQuizzes()
				}
			};
		}

		private List<Quiz> CreateQuizzes()
		{
			_question = new Question
			{
				Statement = "Enter some question here",
				CorrectAnswers = new List<string> { "Yep", "Ok" }
			};

			return new List<Quiz>
			{
				new Quiz
				{
					IsActive = true,
					Questions = new List<Question> { _question },
					MinimumCorrectAnswers = 1
				}
			};
		}

		#endregion
	}
}
