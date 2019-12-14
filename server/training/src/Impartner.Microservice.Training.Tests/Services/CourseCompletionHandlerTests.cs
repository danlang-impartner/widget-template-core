using FluentAssertions;
using Impartner.Microservice.Common.Models;
using Impartner.Microservice.Training.Controllers;
using Impartner.Microservice.Training.Models;
using Impartner.Microservice.Training.Services;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Impartner.Microservice.Training.Tests.Services
{
	[TestFixture]
	public class CourseCompletionHandlerTests : ContextBaseTests
	{
		private CourseCompletionHandler _handler;
		private Mock<IMessenger> _messenger;
		private CertificationStatus _certStatus;
		private CourseStatus _courseStatus;
		private string _objectIdString;

		[SetUp]
		public void SetUp()
		{
			_objectIdString = new ObjectId().ToString();
			_messenger = new Mock<IMessenger>();
			_messenger.Setup(x => x.SendMessage(It.IsAny<string>(), It.IsAny<object>())).ReturnsAsync(true);
			_handler = new CourseCompletionHandler(Repository.Object, _messenger.Object);

			var courses = new List<Course>
			{
				new Course { IsActive = true, CertificationId = _objectIdString },
				new Course { IsActive = true, CertificationId = _objectIdString }
			};
			_certStatus = new CertificationStatus(new Certification(), courses, new User { UserId = UserId });
			_courseStatus = _certStatus.Courses.First();
		}

		#region CompleteCourse

		[Test]
		public async Task should_delegate_to_messenger_to_send_course_completion_object()
		{
			await _handler.CompleteCourse(_certStatus, _courseStatus);

			_messenger.Verify(x => x.SendMessage(CourseCompletionHandler.CourseCompletionName, It.IsAny<object>()));
		}

		[Test]
		public async Task should_not_send_cert_completion_if_there_is_no_course_completion_for_all_courses()
		{
			await _handler.CompleteCourse(_certStatus, _courseStatus);

			_messenger.Verify(
				x => x.SendMessage(CourseCompletionHandler.CertificationCompletionName, It.IsAny<object>()),
				Times.Never());
		}

		[Test]
		public async Task should_send_cert_completion_if_all_courses_are_complete()
		{
			_certStatus.Courses.ForEach(c => c.HasPassed = true);

			await _handler.CompleteCourse(_certStatus, _courseStatus);

			_messenger.Verify(x => x.SendMessage(CourseCompletionHandler.CourseCompletionName, It.IsAny<object>()));
			_messenger.Verify(x => x.SendMessage(
				CourseCompletionHandler.CertificationCompletionName,
				It.Is<object>(arg => ((ExtensionMethods.CertificationCompletion)arg).CreatedBy.UserId == UserId)
			));
		}

		[Test]
		public async Task should_update_is_complete_on_cert_status_if_all_courses_are_complete()
		{
			_certStatus.Courses.ForEach(c => c.HasPassed = true);

			await _handler.CompleteCourse(_certStatus, _courseStatus);

			Repository.Verify(x => x.UpdateAsync(
				CertificationStatusController.CollectionName,
				It.IsAny<Expression<Func<CertificationStatus, bool>>>(),
				_certStatus));
			_certStatus.IsComplete.Should().BeTrue();
			_certStatus.CompletedAt.Should().BeCloseTo(DateTime.UtcNow, 200);
		}

		#endregion
	}
}
