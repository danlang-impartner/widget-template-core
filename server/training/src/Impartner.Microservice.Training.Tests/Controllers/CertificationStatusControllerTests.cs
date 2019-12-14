using FluentAssertions;
using Impartner.Microservice.Common.Exceptions.Http;
using Impartner.Microservice.Common.Models;
using Impartner.Microservice.Training.Controllers;
using Impartner.Microservice.Training.Models;
using Impartner.Microservice.Training.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Moq;
using Neleus.LambdaCompare;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace Impartner.Microservice.Training.Tests.Controllers
{
	[TestFixture]
	public class CertificationStatusControllerTests : ContextBaseTests
	{
		private CertificationStatusController _controller;
		private Mock<ICertificationStatusUpdator> _updator;
		private Mock<ICourseCompletionHandler> _completionHandler;
		private ObjectId _courseId;
		private const string CertificationStatusCollectionName = CertificationStatusController.CollectionName;

		[SetUp]
		public void SetUp()
		{
			_updator = new Mock<ICertificationStatusUpdator>();
			_completionHandler = new Mock<ICourseCompletionHandler>();
			var logger = new Mock<ILogger<CertificationStatusController>>();
			_controller = new CertificationStatusController(Repository.Object, _updator.Object, _completionHandler.Object, logger.Object);
			StubRequest(_controller);
			_courseId = ObjectId.GenerateNewId();
		}

		#region RestartQuiz

		[Test]
		public void should_throw_exception_when_cert_status_id_is_wrong_format()
		{
			_controller.Awaiting(x => x.RestartQuiz("1234", Id, Id))
				.Should().Throw<HttpBadRequestException>();
		}

		[Test]
		public async Task should_call_repo_to_find_cert_status()
		{
			StubCertStatus();
			var objectId = ObjectId.Parse(Id);

			var result = await _controller.RestartQuiz(Id, _courseId.ToString(), Id);

			Repository.Verify(x => x.FindAsync(CertificationStatusCollectionName,
				It.Is<Expression<Func<CertificationStatus, bool>>>(e => Lambda.Eq(e, f => f.Id == objectId))));
		}

		[Test]
		public void should_throw_exception_when_cert_status_id_does_not_exist()
		{
			var certStatusCursor = StubCursor<CertificationStatus>(CertificationStatusCollectionName);
			StubNotFoundResult(certStatusCursor);

			_controller.Awaiting(x => x.RestartQuiz(Id, Id, Id)).Should().Throw<HttpNotFoundException>();
		}

		[Test]
		public async Task should_call_add_new_quiz_status_and_save()
		{
			var objectId = ObjectId.Parse(Id);
			var status = StubCertStatus();

			var result = await _controller.RestartQuiz(Id, _courseId.ToString(), Id);

			Repository.Verify(x => x.UpdateAsync(CertificationStatusCollectionName,
				It.Is<Expression<Func<CertificationStatus, bool>>>(e => Lambda.Eq(e, f => f.Id == objectId)), status));
			result.Data.Should().Be(status);
			status.Courses.First().Quizzes.First().QuizStatuses.Should().HaveCount(2);
		}

		[Test]
		public async Task should_complete_previous_quiz_without_passing_and_add_new_quiz_status()
		{
			var objectId = ObjectId.Parse(Id);
			var status = StubCertStatus();

			var result = await _controller.RestartQuiz(Id, _courseId.ToString(), Id);

			Repository.Verify(x => x.UpdateAsync(CertificationStatusCollectionName,
				It.Is<Expression<Func<CertificationStatus, bool>>>(e => Lambda.Eq(e, f => f.Id == objectId)), status));
			result.Data.Should().Be(status);
			var quizStatuses = status.Courses.First().Quizzes.First().QuizStatuses;
			quizStatuses.Should().HaveCount(2);
			quizStatuses.Count(q => q.IsComplete && !q.HasPassed && q.Status == QuizStatus.Restarted).Should().Be(1);
		}

		#endregion

		#region Patch

		[Test]
		public void should_throw_exception_if_id_is_improper_format_on_patch()
		{
			_controller.Awaiting(x => x.Patch("1234", new JsonPatchDocument<CertificationStatus>()))
				.Should().Throw<HttpBadRequestException>();
		}

		[Test]
		public async Task should_call_repo_to_find_course_status_to_patch()
		{
			var objectId = ObjectId.Parse(Id);
			var status = StubCertStatus();
			_updator.Setup(x => x.Update(status)).Returns(status);

			var result = await _controller.Patch(Id, new JsonPatchDocument<CertificationStatus>());

			Repository.Verify(x => x.FindAsync(CertificationStatusCollectionName,
				It.Is<Expression<Func<CertificationStatus, bool>>>(e => Lambda.Eq(e, f => f.Id == objectId))));
		}

		[Test]
		public void should_throw_exception_when_cert_status_does_not_exist_when_trying_to_patch()
		{
			var certStatusCursor = StubCursor<CertificationStatus>(CertificationStatusCollectionName);
			StubNotFoundResult(certStatusCursor);

			_controller.Awaiting(x => x.Patch(Id, new JsonPatchDocument<CertificationStatus>()))
				.Should().Throw<HttpNotFoundException>();
		}

		[Test]
		public void should_throw_exception_when_cert_status_has_been_completed()
		{
			var status = StubCertStatus();
			status.IsComplete = true;

			_controller.Awaiting(x => x.Patch(Id, new JsonPatchDocument<CertificationStatus>()))
				.Should().Throw<HttpStatusCodeException>()
				.Which.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
		}

		[Test]
		public async Task should_delegate_to_course_status_updator_to_update_test_results()
		{
			var status = StubCertStatus();
			_updator.Setup(x => x.Update(status)).Returns(status);

			var result = await _controller.Patch(Id, new JsonPatchDocument<CertificationStatus>());

			_updator.Verify(u => u.Update(status));
		}

		[Test]
		public async Task should_delegate_to_repo_to_persist_updated_status()
		{
			var objectId = ObjectId.Parse(Id);
			var status = StubCertStatus();
			_updator.Setup(x => x.Update(status)).Returns(status);

			var result = await _controller.Patch(Id, new JsonPatchDocument<CertificationStatus>());

			Repository.Verify(x => x.UpdateAsync(CertificationStatusCollectionName,
				It.Is<Expression<Func<CertificationStatus, bool>>>(e => Lambda.Eq(e, f => f.Id == objectId)), status));
			result.Value.Should().Be(status);
			_completionHandler.Verify(x => x.CompleteCourse(status, status.Courses.First()), Times.Never());
		}

		[Test]
		public async Task should_delegate_to_completion_handler_if_course_is_passed()
		{
			var certStatus = StubCertStatus();
			var returnedCertStatus = CreateCertStatus();
			_updator.Setup(x => x.Update(certStatus)).Returns(returnedCertStatus);
			var courseStatus = returnedCertStatus.Courses.First();
			courseStatus.HasPassed = true;

			var result = await _controller.Patch(Id, new JsonPatchDocument<CertificationStatus>());

			_completionHandler.Verify(x => x.CompleteCourse(returnedCertStatus, courseStatus));
		}

		#endregion

		#region Private methods

		private CertificationStatus StubCertStatus()
		{
			var certStatus = CreateCertStatus();
			var certStatusCursor = StubCursor<CertificationStatus>(CertificationStatusCollectionName);
			StubSingleFoundResult(certStatusCursor, certStatus);
			return certStatus;
		}

		private CertificationStatus CreateCertStatus()
		{
			var courses = new List<Course>
			{
				new Course
				{
					Id = _courseId,
					CertificationId = Id,
					IsActive = true,
					Quizzes = new List<Quiz>{ new Quiz { Id = ObjectId.Parse(Id), IsActive = true } }
				}
			};
			return new CertificationStatus(new Certification(), courses, new User { UserId = UserId });
		}

		#endregion
	}
}
