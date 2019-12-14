using FluentAssertions;
using Impartner.Microservice.Common.Models;
using Impartner.Microservice.Training.Models;
using Impartner.Microservice.Training.Services;
using MongoDB.Bson;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Impartner.Microservice.Training.Tests.Extensions
{
	[TestFixture]
	public class TrainingObjectExtensionsTests : ContextBaseTests
	{
		private CertificationStatus _certStatus;
		private CourseStatus _courseStatus;
		private User _user;
		private ObjectId _objectId;
		private const int VersionNumber = 3;

		[SetUp]
		public void SetUp()
		{
			_objectId = new ObjectId();
			_user = new User
			{
				UserId = UserId,
				Username = Username,
				FirstName = FirstName,
				LastName = LastName
			};

			var cert = CreateCertification();
			var courses = CreateCourses();
			_certStatus = new CertificationStatus(cert, courses, _user);
			_courseStatus = _certStatus.Courses.First();
		}

		[Test]
		public void should_generate_full_course_completion_object_from_course_status()
		{
			_courseStatus.CompletedAt = DateTime.UtcNow;

			var completion = _courseStatus.ToCourseCompletion();

			completion.CourseVersion.Should().Be(_courseStatus.CourseVersion);
			completion.ProgramId.Should().Be(_courseStatus.TenantId);
			completion.CertificationId.Should().Be(_courseStatus.CertificationId);
			completion.CourseId.Should().Be(_courseStatus.CourseId);
			completion.NumberOfAttempts.Should().Be(2);
			completion.CompletedAt.Should().Be(_courseStatus.CompletedAt ?? DateTime.UtcNow);
			completion.CreatedBy.Should().Be(_user);
			completion.Name.Should().Be(_courseStatus.Name);
			completion.Description.Should().Be(_courseStatus.Description);
		}

		[Test]
		public void should_generate_full_certification_completion_object_from_cert_status()
		{
			var completion = _certStatus.ToCertificationCompletion();

			completion.CertificationVersion.Should().Be(_certStatus.CertificationVersion);
			completion.ProgramId.Should().Be(_certStatus.TenantId);
			completion.Id.Should().Be(_certStatus.CertificationId);
			completion.Name.Should().Be(_certStatus.Name);
			completion.Description.Should().Be(_certStatus.Description);
			completion.CreatedBy.Should().Be(_user);
			completion.CompletedAt.Should().BeCloseTo(DateTime.UtcNow, 200);
			completion.Expiration.Should().BeCloseTo(DateTime.UtcNow.AddMonths(12), 200);
		}

		#region Helper methods

		private Certification CreateCertification()
		{
			return new Certification
			{
				TenantId = TenantId,
				Id = _objectId,
				CertificationVersion = VersionNumber,
				Expiration = 12,
				Name = "My Cert",
				Description = "I have been described"
			};
		}

		private IEnumerable<Course> CreateCourses()
		{
			return new List<Course>
			{
				new Course
				{
					TenantId = TenantId,
					CourseVersion = VersionNumber,
					CertificationId = _objectId.ToString(),
					Id = _objectId,
					IsActive = true,
					Name = "name",
					Description = "describings...",
					Quizzes = CreateQuizzes()
				}
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
	}
}
