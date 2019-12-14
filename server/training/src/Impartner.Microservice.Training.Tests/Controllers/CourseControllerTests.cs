using FluentAssertions;
using Impartner.Microservice.Common.Exceptions;
using Impartner.Microservice.Common.Exceptions.Http;
using Impartner.Microservice.Common.Models;
using Impartner.Microservice.Training.Controllers;
using Impartner.Microservice.Training.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using Neleus.LambdaCompare;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Impartner.Microservice.Training.Tests.Controllers
{
	[TestFixture]
	public class CourseControllerTests : ContextBaseTests
	{
		private CourseController _controller;
		private List<Course> _data;
		private Mock<IFindFluent<Course, Course>> _findCursor;
		private Mock<IAsyncCursor<Course>> _asyncCursor;
		private const string CourseCollectionName = CourseController.CollectionName;
		private const string CertCollectionName = CertificationController.CollectionName;
		private const string CertId = "5cf990d822ccb41e08181429";

		[SetUp]
		public void SetUp()
		{
			_controller = new CourseController(Repository.Object);
			StubRequest(_controller);
			StubCursors();
		}

		private void StubCursors()
		{
			_data = new List<Course>();
			for (var i = 0; i < 5; i++)
				_data.Add(new Course { Name = "test" + i });
			_findCursor = new Mock<IFindFluent<Course, Course>>();
			_findCursor.Setup(x => x.Skip(It.IsAny<int>())).Returns(_findCursor.Object);
			_findCursor.Setup(x => x.Limit(It.IsAny<int>())).Returns(_findCursor.Object);
			_asyncCursor = new Mock<IAsyncCursor<Course>>();
			_findCursor.Setup(x => x.ToCursorAsync(It.IsAny<CancellationToken>())).ReturnsAsync(_asyncCursor.Object);
			_asyncCursor.Setup(x => x.Current).Returns(_data);
			var seq = _asyncCursor.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>()));
			var asyncSeq = _asyncCursor.SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>()));
			for (var i = 0; i < 5; i++)
			{
				seq.Returns(true);
				asyncSeq.Returns(Task.FromResult(true));
			}
			seq.Returns(false);
			asyncSeq.Returns(Task.FromResult(false));

			Repository.Setup(x => x.Find(CourseCollectionName, It.IsAny<FilterDefinition<Course>>())).Returns(_findCursor.Object);
		}

		#region Get

		[Test]
		public async Task should_call_find_on_collection_returning_all_results()
		{
			var result = await ExecuteEndpoint(_controller.Get());

			Repository.Verify(x => x.Find(CourseCollectionName, It.IsAny<FilterDefinition<Course>>()));
			result.Should().HaveCount(25);
			_findCursor.Verify(x => x.Skip(0));
			_findCursor.Verify(x => x.Limit(100));
		}

		[Test]
		public async Task should_add_skip_and_limit_when_specified()
		{
			const int numberOfResults = 5;
			const int skip = 20;

			await ExecuteEndpoint(_controller.Get(take: numberOfResults, skip: skip));

			_findCursor.Verify(x => x.Skip(skip));
			_findCursor.Verify(x => x.Limit(numberOfResults));
		}

		#endregion

		#region Get(string id)

		[Test]
		public void should_provide_count_of_active_lessons_and_quizzes()
		{
			var course = new Course
			{
				Lessons = new List<Lesson>
				{
					new Lesson { IsActive = true },
					new Lesson { IsActive = false }
				},
				Quizzes = new List<Quiz>
				{
					new Quiz { IsActive = true }
				}
			};

			course.ActiveLessonCount.Should().Be(1);
			course.ActiveQuizCount.Should().Be(1);
		}

		[Test]
		public async Task should_delegate_to_find_results_with_expected_params()
		{
			var objectId = ObjectId.Parse(Id);
			var cursor = StubCursor<Course>(CourseCollectionName);
			var course = new Course();
			StubSingleFoundResult(cursor, course);

			var response = await ExecuteEndpoint(_controller.GetById(Id));

			AssertRepositoryWasQueriedById(objectId);
			response.Should().Be(course);
		}

		[Test]
		public void should_throw_object_not_found_exception_404_when_no_result_in_database()
		{
			var cursor = StubCursor<Course>(CourseCollectionName);
			StubNotFoundResult(cursor);

			var response = _controller.Awaiting(x => ExecuteEndpoint(x.GetById(Id)));

			AssertObjectNotFoundExceptionThrown(response);
		}

		#endregion

		#region AddNewItem

		[Test]
		public async Task should_delegate_to_repo_to_save_course_and_denormalized_data()
		{
			var certId = ObjectId.Parse(CertId);
			var course = new Course { CertificationId = CertId, Name = "Course Name", IsActive = true };
			var cert = new Certification { Id = certId, Courses = new List<DenormalizedCourse> { new DenormalizedCourse { Id = ObjectId.Empty } } };
			var cursor = StubCursor<Certification>(CertCollectionName);
			StubSingleFoundResult(cursor, cert);
			Repository.Setup(x => x.SaveAsync(CourseCollectionName, course))
				.ReturnsAsync(course);

			var response = await ExecuteEndpoint(_controller.AddNewItem(course));

			// verify course
			Repository.Verify(x => x.SaveAsync(CourseCollectionName, course));
			response.Should().Be(course);
			response.CreatedBy.UserId.Should().BeEquivalentTo(UserId);
			response.UpdatedBy.UserId.Should().BeEquivalentTo(UserId);
			response.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, 500);
			response.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, 500);
			// verify denormalized data
			Repository.Verify(x => x.UpdateAsync(CertCollectionName,
				It.Is<Expression<Func<Certification, bool>>>(e => Lambda.Eq(e, f => f.Id == certId)), It.IsAny<Certification>()));
			cert.Courses.Count.Should().Be(2);
			cert.Courses.Count(x => x.Id != ObjectId.Empty).Should().Be(1);
		}

		#endregion

		#region Patch

		[Test]
		public async Task should_delegate_to_repo_to_lookup_by_id()
		{
			var objectId = ObjectId.Parse(Id);
			var cursor = StubCursor<Course>(CourseCollectionName);
			var course = new Course();
			StubSingleFoundResult(cursor, course);
			var patch = new JsonPatchDocument<Course>();
			Repository.Setup(x => x.UpdateAsync(CourseCollectionName, It.IsAny<Expression<Func<Course, bool>>>(), course))
				.ReturnsAsync(new ReplaceOneResult.Acknowledged(1, 1, objectId));

			await ExecuteEndpoint(_controller.Patch(Id, patch));

			AssertRepositoryWasQueriedById(objectId);
		}

		[Test]
		public void should_throw_object_not_found_exception_404_when_no_object_found_by_id()
		{
			var cursor = StubCursor<Course>(CourseCollectionName);
			StubNotFoundResult(cursor);
			var patch = new JsonPatchDocument<Course>();

			var response = _controller.Awaiting(x => ExecuteEndpoint(x.Patch(Id, patch)));

			AssertObjectNotFoundExceptionThrown(response);
		}

		[Test]
		public async Task should_apply_patch_then_delegate_to_repo_to_update_course_data()
		{
			var objectId = ObjectId.Parse(Id);
			var cursor = StubCursor<Course>(CourseCollectionName);
			var course = new Course { UpdatedBy = new User { UserId = "4" }, UpdatedAt = DateTime.UtcNow.AddDays(-3) };
			StubSingleFoundResult(cursor, course);
			var patch = new JsonPatchDocument<Course>();
			Repository.Setup(x => x.UpdateAsync(CourseCollectionName, It.IsAny<Expression<Func<Course, bool>>>(), course))
				.ReturnsAsync(new ReplaceOneResult.Acknowledged(1, 1, objectId));

			var response = await ExecuteEndpoint(_controller.Patch(Id, patch));

			Repository.Verify(x => x.UpdateAsync(CourseCollectionName,
				It.Is<Expression<Func<Course, bool>>>(e => Lambda.Eq(e, f => f.Id == objectId)), course));
			response.UpdatedBy.UserId.Should().BeEquivalentTo(UserId);
			response.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, 500);
		}

		[Test]
		public async Task should_apply_patch_and_update_denormalized_certification_data_when_name_or_isactive_change()
		{
			const string originalName = "Course With No Name";
			const string newName = "Course Now Has A Name";
			var objectId = ObjectId.Parse(Id);
			var certId = ObjectId.Parse(CertId);
			var courseCursor = StubCursor<Course>(CourseCollectionName);
			var certificationCursor = StubCursor<Certification>(CertCollectionName);
			var course = new Course { Id = objectId, CertificationId = CertId, Name = originalName };
			var cert = new Certification { Id = certId, Courses = new List<DenormalizedCourse> { new DenormalizedCourse { Id = objectId, Name = originalName } } };
			StubSingleFoundResult(courseCursor, course);
			StubSingleFoundResult(certificationCursor, cert);
			var patch = new JsonPatchDocument<Course>(new List<Operation<Course>> { new Operation<Course>("replace", "name", "", newName) }, new CamelCasePropertyNamesContractResolver());
			Repository.Setup(x => x.UpdateAsync(CourseCollectionName, It.IsAny<Expression<Func<Course, bool>>>(), course))
				.ReturnsAsync(new ReplaceOneResult.Acknowledged(1, 1, objectId));

			var response = await _controller.Patch(Id, patch);

			response.Data.Id.Should().Be(objectId);
			Repository.Verify(x => x.UpdateAsync(CourseCollectionName,
				It.Is<Expression<Func<Course, bool>>>(e => Lambda.Eq(e, f => f.Id == objectId)), course));
			Repository.Verify(x => x.UpdateAsync(CertCollectionName,
				It.Is<Expression<Func<Certification, bool>>>(e => Lambda.Eq(e, f => f.Id == certId)), It.IsAny<Certification>()));
			cert.Courses.Single(x => x.Id == objectId).Name.Should().Be(newName);
		}

		[Test]
		public void should_throw_invalid_json_patch_exception_422_when_patch_is_invalid()
		{
			var course = new Course();
			var courseCursor = StubCursor<Course>(CourseCollectionName);
			StubSingleFoundResult(courseCursor, course);
			var patch = new JsonPatchDocument<Course>(new List<Operation<Course>> { new Operation<Course>("replace", "named", "", null) }, new CamelCasePropertyNamesContractResolver());

			var response = _controller.Awaiting(x => x.Patch(Id, patch));

			response.Should()
				.ThrowExactly<InvalidJsonPatchException>()
				.Which.StatusCode
				.Should().Be((int)HttpStatusCode.UnprocessableEntity);
		}

		[Test]
		public void should_throw_bad_request_exception_400_when_updating_an_active_course()
		{
			var course = new Course { IsActive = true };
			var cursor = StubCursor<Course>(CourseCollectionName);
			StubSingleFoundResult(cursor, course);
			var patch = new JsonPatchDocument<Course>();

			var response = _controller.Awaiting(x => x.Patch(Id, patch));

			response.Should()
				.ThrowExactly<HttpBadRequestException>()
				.Which.StatusCode
				.Should().Be((int)HttpStatusCode.BadRequest);
		}

		[Test]
		public void should_throw_bad_request_exception_when_activating_course_with_no_active_lesson()
		{
			var course = new Course { IsActive = false, Quizzes = new List<Quiz> { new Quiz { IsActive = true } } };
			var cursor = StubCursor<Course>(CourseCollectionName);
			StubSingleFoundResult(cursor, course);
			var patch = new JsonPatchDocument<Course>();
			var op = new Operation<Course>("replace", "isActive", "", true);
			patch.Operations.Add(op);

			_controller.Awaiting(x => x.Patch(Id, patch))
				.Should().ThrowExactly<HttpBadRequestException>()
				.Which.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		[Test]
		public void should_throw_bad_request_exception_when_activating_course_with_no_active_quiz()
		{
			var course = new Course { IsActive = false, Lessons = new List<Lesson> { new Lesson { IsActive = true } } };
			var cursor = StubCursor<Course>(CourseCollectionName);
			StubSingleFoundResult(cursor, course);
			var patch = new JsonPatchDocument<Course>();
			var op = new Operation<Course>("replace", "isActive", "", true);
			patch.Operations.Add(op);

			_controller.Awaiting(x => x.Patch(Id, patch))
				.Should().ThrowExactly<HttpBadRequestException>()
				.Which.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		[Test]
		public void should_throw_bad_request_exception_when_activating_course_with_active_quiz_with_question_with_no_correct_answers()
		{
			var course = new Course
			{
				IsActive = false,
				Lessons = new List<Lesson> { new Lesson { IsActive = true } },
				Quizzes = new List<Quiz>
				{
					new Quiz { IsActive = true, Questions = new List<Question> { new Question { CorrectAnswers = new List<string> { "1", "2" } } } },
					new Quiz { IsActive = true, Questions = new List<Question> { new Question { CorrectAnswers = new List<string>() } } }
				}
			};
			var cursor = StubCursor<Course>(CourseCollectionName);
			StubSingleFoundResult(cursor, course);
			var patch = new JsonPatchDocument<Course>();
			var op = new Operation<Course>("replace", "isActive", "", true);
			patch.Operations.Add(op);

			_controller.Awaiting(x => x.Patch(Id, patch))
				.Should().ThrowExactly<HttpBadRequestException>()
				.Which.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		[Test]
		public async Task should_increment_version_number_when_activating_course()
		{
			const int versionNumber = 3;
			var course = new Course
			{
				CertificationId = CertId,
				IsActive = false,
				CourseVersion = versionNumber,
				Lessons = new List<Lesson> { new Lesson { IsActive = true } },
				Quizzes = new List<Quiz> { new Quiz { IsActive = true } }
			};
			var cursor = StubCursor<Course>(CourseCollectionName);
			StubSingleFoundResult(cursor, course);
			var patch = new JsonPatchDocument<Course>();
			var op = new Operation<Course>("replace", "isActive", "", true);
			patch.Operations.Add(op);
			var certificationCursor = StubCursor<Certification>(CertCollectionName);
			var cert = new Certification { Courses = new List<DenormalizedCourse> { new DenormalizedCourse() } };
			StubSingleFoundResult(certificationCursor, cert);

			var response = await _controller.Patch(Id, patch);

			response.Data.CourseVersion.Should().Be(versionNumber + 1);
		}

		#endregion

		#region Delete

		[Test]
		public async Task should_delegate_to_repo_to_delete_document_and_update_denormalized_data_in_certification()
		{
			var objectId = ObjectId.Parse(Id);
			var certId = ObjectId.Parse(CertId);
			var courseCursor = StubCursor<Course>(CourseCollectionName);
			var certificationCursor = StubCursor<Certification>(CertCollectionName);
			var course = new Course { Id = objectId, CertificationId = CertId };
			var cert = new Certification { Id = certId, Courses = new List<DenormalizedCourse> { new DenormalizedCourse { Id = objectId }, new DenormalizedCourse { Id = ObjectId.Empty } } };
			StubSingleFoundResult(courseCursor, course);
			StubSingleFoundResult(certificationCursor, cert);
			Repository.Setup(x => x.DeleteAsync(CourseCollectionName, It.IsAny<Expression<Func<Course, bool>>>()))
				.ReturnsAsync(new DeleteResult.Acknowledged(1));

			await _controller.Delete(Id);

			Repository.Verify(x => x.DeleteAsync(CourseCollectionName,
				It.Is<Expression<Func<Course, bool>>>(e => Lambda.Eq(e, f => f.Id == objectId))));
			Repository.Verify(x => x.UpdateAsync(CertCollectionName,
				It.Is<Expression<Func<Certification, bool>>>(e => Lambda.Eq(e, f => f.Id == certId)), It.IsAny<Certification>()));
			cert.Courses.Count.Should().Be(1);
		}

		#endregion

		#region Private methods

		private void AssertRepositoryWasQueriedById(ObjectId objectId)
		{
			Repository.Verify(x => x.FindAsync(CourseCollectionName,
				It.Is<Expression<Func<Course, bool>>>(e => Lambda.Eq(e, f => f.Id == objectId))));
		}

		private static void AssertObjectNotFoundExceptionThrown(Func<Task> func)
		{
			func.Should()
				.ThrowExactly<HttpNotFoundException>()
				.Which.StatusCode
				.Should().Be((int)HttpStatusCode.NotFound);
		}

		#endregion
	}
}
