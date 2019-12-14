using FluentAssertions;
using Impartner.Microservice.Common.Models;
using Impartner.Microservice.Training.Models;
using MongoDB.Bson;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Impartner.Microservice.Training.Tests.Models
{
	[TestFixture]
	public class CertificationStatusTests
	{
		private Certification _cert;
		private List<Course> _courses;
		private User _user;
		private const int TenantId = 40;

		[SetUp]
		public void SetUp()
		{
			_user = new User
			{
				FirstName = "First",
				LastName = "Last",
				UserId = "123456",
				Username = "username"
			};
			_cert = CreateCertification();
			_courses = CreateCourses();
		}

		#region Helper methods

		private Certification CreateCertification()
		{
			return new Certification
			{
				TenantId = TenantId,
				Id = new ObjectId(),
				CertificationVersion = 2,
				CreatedBy = _user,
				Expiration = 0,
				Language = "English",
				Name = "My Cert",
				Description = "I have been described",
				IsActive = true
			};
		}

		private List<Course> CreateCourses()
		{
			return new List<Course>
			{
				new Course
				{
					TenantId = TenantId,
					IsActive = true,
					CertificationId = _cert.Id.ToString(),
					Name = "name",
					CreatedBy = _user,
					Lessons = CreateLessons(),
					Quizzes = CreateQuizzes()
				},
				new Course
				{
					TenantId = TenantId,
					IsActive = true,
					CertificationId = _cert.Id.ToString(),
					Name = "name",
					CreatedBy = _user,
					Lessons = CreateLessons(),
					Quizzes = CreateQuizzes()
				}
			};
		}

		private List<Lesson> CreateLessons()
		{
			return new List<Lesson>
			{
				new Lesson { IsActive = true },
				new Lesson { IsActive = true }
			};
		}

		private List<Quiz> CreateQuizzes()
		{
			return new List<Quiz>
			{
				new Quiz{ IsActive = true },
				new Quiz{ IsActive = true }
			};
		}

		#endregion

		#region Constructor

		[Test]
		public void should_initialize_base_properties_from_cert()
		{
			var certStatus = new CertificationStatus(_cert, _courses, _user);

			certStatus.TenantId.Should().Be(_cert.TenantId);
			certStatus.CertificationId.Should().Be(_cert.Id);
			certStatus.CertificationVersion.Should().Be(_cert.CertificationVersion);
			certStatus.Name.Should().Be(_cert.Name);
			certStatus.Description.Should().Be(_cert.Description);
			certStatus.CreatedBy.Should().Be(_user);
			certStatus.Language.Should().Be(_cert.Language);
			certStatus.Expiration.Should().Be(_cert.Expiration);
		}

		[Test]
		public void should_initialize_base_properties_from_course()
		{
			var certStatus = new CertificationStatus(_cert, _courses, _user);

			var course = _courses.First();
			var courseStatus = certStatus.Courses.First();
			courseStatus.TenantId.Should().Be(course.TenantId);
			courseStatus.CertificationId.Should().Be(ObjectId.Parse(course.CertificationId));
			courseStatus.CourseId.Should().Be(course.Id);
			courseStatus.Name.Should().Be(course.Name);
			courseStatus.Description.Should().Be(course.Description);
			courseStatus.CreatedBy.Should().Be(_user);
		}

		[Test]
		public void should_add_each_active_course_from_course_list()
		{
			_courses.Last().IsActive = false;

			var certStatus = new CertificationStatus(_cert, _courses, _user);

			_courses.Count.Should().BeGreaterThan(certStatus.Courses.Count);
			certStatus.Courses.Count.Should().Be(_courses.Count - 1);
		}

		[Test]
		public void should_add_each_active_lesson_from_course()
		{
			_courses.First().Lessons.Last().IsActive = false;

			var certStatus = new CertificationStatus(_cert, _courses, _user);

			var inputLessonCount = _courses.First().Lessons.Count;
			inputLessonCount.Should().BeGreaterThan(certStatus.Courses.First().Lessons.Count);
			certStatus.Courses.First().Lessons.Count.Should().Be(inputLessonCount - 1);
		}

		[Test]
		public void should_add_each_active_quiz_from_course()
		{
			_courses.First().Quizzes.Last().IsActive = false;

			var certStatus = new CertificationStatus(_cert, _courses, _user);

			var inputQuizCount = _courses.First().Quizzes.Count;
			inputQuizCount.Should().BeGreaterThan(certStatus.Courses.First().Quizzes.Count);
			certStatus.Courses.First().Quizzes.Count.Should().Be(inputQuizCount - 1);
		}

		[Test]
		public void should_add_initial_quiz_status_on_creation()
		{
			var certStatus = new CertificationStatus(_cert, _courses, _user);

			foreach (var course in certStatus.Courses)
			{
				foreach (var quiz in course.Quizzes)
				{
					quiz.QuizStatuses.Count.Should().Be(1);
				}
			}
		}

		#endregion

		#region RestartQuiz

		[Test]
		public void should_add_new_quiz_status_when_none_exist()
		{
			var certStatus = new CertificationStatus(_cert, _courses, _user);
			var courseStatus = certStatus.Courses.First();
			var quiz = courseStatus.Quizzes.First();
			quiz.QuizStatuses.Clear();

			courseStatus.RestartQuiz(quiz);

			courseStatus.Quizzes.First().QuizStatuses.Should().HaveCount(1);
		}

		[Test]
		public void should_close_old_quiz_status_and_add_new_quiz_status_when_one_already_exists()
		{
			var certStatus = new CertificationStatus(_cert, _courses, _user);
			var courseStatus = certStatus.Courses.First();
			var quiz = courseStatus.Quizzes.First();

			courseStatus.RestartQuiz(quiz);

			courseStatus.Quizzes.First().QuizStatuses.Should().HaveCount(2);
			var oldQuiz = courseStatus.Quizzes.First().QuizStatuses.First();
			oldQuiz.Status.Should().Be(QuizStatus.Restarted);
			oldQuiz.IsComplete.Should().BeTrue();
			oldQuiz.HasPassed.Should().BeFalse();
			var newQuiz = courseStatus.Quizzes.First().QuizStatuses.Last();
			newQuiz.Status.Should().Be(QuizStatus.NotStarted);
			newQuiz.IsComplete.Should().BeFalse();
			newQuiz.HasPassed.Should().BeFalse();
		}

		#endregion
	}
}
