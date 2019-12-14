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
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Impartner.Microservice.Training.Tests.Controllers
{
	[TestFixture]
	public class CertificationControllerTests : ContextBaseTests
	{
		private CertificationController _controller;
		private List<Certification> _data;
		private Mock<IFindFluent<Certification, Certification>> _findCursor;
		private Mock<IAsyncCursor<Certification>> _asyncCursor;
		private const string CertCollectionName = CertificationController.CollectionName;
		private const string CourseCollectionName = CourseController.CollectionName;
		private const string CertificationStatusCollectionName = CertificationStatusController.CollectionName;
		private const string CertId = "5cf990d822ccb41e08181429";

		[SetUp]
		public void SetUp()
		{
			_controller = new CertificationController(Repository.Object);
			StubRequest(_controller);
			StubCursors();
		}

		private void StubCursors()
		{
			_data = new List<Certification>();
			for (var i = 0; i < 5; i++)
				_data.Add(new Certification { Name = "test" + i });
			_findCursor = new Mock<IFindFluent<Certification, Certification>>();
			_findCursor.Setup(x => x.Skip(It.IsAny<int>())).Returns(_findCursor.Object);
			_findCursor.Setup(x => x.Limit(It.IsAny<int>())).Returns(_findCursor.Object);
			_asyncCursor = new Mock<IAsyncCursor<Certification>>();
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

			Repository.Setup(x => x.Find(CertCollectionName, It.IsAny<FilterDefinition<Certification>>())).Returns(_findCursor.Object);
		}

		#region Get

		[Test]
		public async Task should_call_find_on_collection_returning_all_results()
		{
			var result = await ExecuteEndpoint(_controller.Get());

			Repository.Verify(x => x.Find(CertCollectionName, It.IsAny<FilterDefinition<Certification>>()));
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
		public async Task should_delegate_to_find_results_with_expected_params()
		{
			var objectId = ObjectId.Parse(Id);
			var cursor = StubCursor<Certification>(CertCollectionName);
			var cert = new Certification();
			StubSingleFoundResult(cursor, cert);

			var response = await ExecuteEndpoint(_controller.GetById(Id));

			AssertRepositoryWasQueriedById(objectId);
			response.Should().Be(cert);
		}

		[Test]
		public void should_return_not_found_404_when_no_result_in_database()
		{
			var cursor = StubCursor<Certification>(CertCollectionName);
			StubNotFoundResult(cursor);

			_controller.Awaiting(x => ExecuteEndpoint(x.GetById(Id)))
				.Should().ThrowExactly<HttpNotFoundException>().And
				.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		#endregion

		#region CreateStatus

		[Test]
		public void should_throw_exception_when_cert_status_already_exists()
		{
			var certId = new ObjectId();
			var certStatusCursor = StubCursor<CertificationStatus>(CertificationStatusController.CollectionName);
			var status = new CertificationStatus { CertificationId = certId };
			StubSingleFoundResult(certStatusCursor, status);

			_controller.Awaiting(x => x.CreateStatus(certId.ToString())).Should().Throw<HttpBadRequestException>()
				.And.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		[Test]
		public async Task should_call_repo_to_save_new_cert_status_for_the_user_when_cert_is_valid()
		{
			var certStatusCursor = StubCursor<CertificationStatus>(CertificationStatusCollectionName);
			StubNotFoundResult(certStatusCursor);
			var certCursor = StubCursor<Certification>(CertCollectionName);
			var cert = new Certification { IsActive = true };
			StubSingleFoundResult(certCursor, cert);
			var courseCursor = StubCursor<Course>(CourseCollectionName);
			var course = new Course { CertificationId = CertId, IsActive = true };
			StubSingleFoundResult(courseCursor, course);
			var status = new CertificationStatus();
			Repository.Setup(x => x.SaveAsync(CertificationStatusCollectionName, It.IsAny<CertificationStatus>()))
				.ReturnsAsync(status);

			var result = await _controller.CreateStatus(Id);

			result.Data.Should().Be(status);
			Repository.Verify(x => x.SaveAsync(CertificationStatusCollectionName, It.IsAny<CertificationStatus>()));
		}

		[Test]
		public void should_throw_exception_when_certification_is_not_active()
		{
			var certCursor = StubCursor<Certification>(CertCollectionName);
			var cert = new Certification { IsActive = false };
			StubSingleFoundResult(certCursor, cert);
			var certStatusCursor = StubCursor<CertificationStatus>(CertificationStatusCollectionName);
			StubNotFoundResult(certStatusCursor);

			_controller.Awaiting(x => x.CreateStatus(Id)).Should().Throw<HttpStatusCodeException>().And.StatusCode.Should()
				.Be(HttpStatusCode.MethodNotAllowed);
		}

		[Test]
		public void should_throw_exception_when_certification_has_no_active_courses()
		{
			var certStatusCursor = StubCursor<CertificationStatus>(CertificationStatusCollectionName);
			StubNotFoundResult(certStatusCursor);
			var certCursor = StubCursor<Certification>(CertCollectionName);
			var cert = new Certification { IsActive = false };
			StubSingleFoundResult(certCursor, cert);
			var courseCursor = StubCursor<Course>(CourseCollectionName);
			StubNotFoundResult(courseCursor);

			_controller.Awaiting(x => x.CreateStatus(Id)).Should().Throw<HttpStatusCodeException>().And.StatusCode.Should()
				.Be(HttpStatusCode.MethodNotAllowed);
		}

		#endregion

		#region GetStatus

		[Test]
		public void should_throw_exception_when_cert_id_is_wrong_format()
		{
			_controller.Awaiting(x => x.GetStatus("1234")).Should().Throw<HttpBadRequestException>();
		}

		[Test]
		public async Task should_call_repo_to_find_cert_statuses()
		{
			var objectId = ObjectId.Parse(Id);
			var certStatusCursor = StubCursor<CertificationStatus>(CertificationStatusCollectionName);
			var certStatus1 = new CertificationStatus();
			var certStatus2 = new CertificationStatus { IsComplete = true };
			StubListOfResult(certStatusCursor, new List<CertificationStatus> { certStatus1, certStatus2 });

			var result = await _controller.GetStatus(Id);

			Repository.Verify(x => x.FindAsync(CertificationStatusCollectionName,
				It.Is<Expression<Func<CertificationStatus, bool>>>(e => Lambda.Eq(e, f => f.CertificationId == objectId && f.CreatedBy.UserId == UserId))));
			result.Data.Count.Should().Be(2);
		}

		#endregion

		#region AddNewItem

		[Test]
		public async Task should_delegate_to_repo_to_save_certification()
		{
			var cert = new Certification();
			Repository.Setup(x => x.SaveAsync(CertCollectionName, cert))
				.ReturnsAsync(cert);

			var response = await ExecuteEndpoint(_controller.AddNewItem(cert));

			Repository.Verify(x => x.SaveAsync(CertCollectionName, cert));
			response.Should().Be(cert);
			response.CreatedBy.UserId.Should().BeEquivalentTo(UserId);
			response.UpdatedBy.UserId.Should().BeEquivalentTo(UserId);
			response.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, 500);
			response.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, 500);
		}

		#endregion

		#region Patch

		[Test]
		public async Task should_delegate_to_repo_to_lookup_by_id()
		{
			var objectId = ObjectId.Parse(Id);
			var cursor = StubCursor<Certification>(CertCollectionName);
			var cert = new Certification();
			StubSingleFoundResult(cursor, cert);
			var patch = new JsonPatchDocument<Certification>();
			Repository.Setup(x => x.UpdateAsync(CertCollectionName, It.IsAny<Expression<Func<Certification, bool>>>(), cert))
				.ReturnsAsync(new ReplaceOneResult.Acknowledged(1, 1, objectId));

			await ExecuteEndpoint(_controller.Patch(Id, patch));

			AssertRepositoryWasQueriedById(objectId);
		}

		[Test]
		public void should_return_not_found_result_when_no_object_found_by_id()
		{
			var cursor = StubCursor<Certification>(CertCollectionName);
			StubNotFoundResult(cursor);
			var patch = new JsonPatchDocument<Certification>();

			_controller.Awaiting(x => ExecuteEndpoint(x.Patch(Id, patch)))
				.Should().ThrowExactly<HttpNotFoundException>().And
				.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Test]
		public async Task should_apply_patch_then_delegate_to_repo_to_update_certification_data()
		{
			var objectId = ObjectId.Parse(Id);
			var cursor = StubCursor<Certification>(CertCollectionName);
			var cert = new Certification { UpdatedBy = new User { UserId = "4" }, UpdatedAt = DateTime.UtcNow.AddDays(-3) };
			StubSingleFoundResult(cursor, cert);
			var patch = new JsonPatchDocument<Certification>();
			Repository.Setup(x => x.UpdateAsync(CertCollectionName, It.IsAny<Expression<Func<Certification, bool>>>(), cert))
				.ReturnsAsync(new ReplaceOneResult.Acknowledged(1, 1, objectId));

			var response = await ExecuteEndpoint(_controller.Patch(Id, patch));

			Repository.Verify(x => x.UpdateAsync(CertCollectionName,
				It.Is<Expression<Func<Certification, bool>>>(e => Lambda.Eq(e, f => f.Id == objectId)), cert));
			response.UpdatedBy.UserId.Should().BeEquivalentTo(UserId);
			response.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, 500);
		}

		[Test]
		public void should_throw_invalid_json_patch_exception_422_when_patch_is_invalid()
		{
			var cert = new Certification();
			var cursor = StubCursor<Certification>(CertCollectionName);
			StubSingleFoundResult(cursor, cert);
			var patch = new JsonPatchDocument<Certification>(new List<Operation<Certification>> { new Operation<Certification>("replace", "named", "", null) }, new CamelCasePropertyNamesContractResolver());

			var response = _controller.Awaiting(x => x.Patch(Id, patch));

			response.Should()
				.ThrowExactly<InvalidJsonPatchException>()
				.Which.StatusCode
				.Should().Be((int)HttpStatusCode.UnprocessableEntity);
		}

		[Test]
		public void should_throw_bad_request_exception_when_updating_an_active_cert()
		{
			var cert = new Certification { IsActive = true };
			var cursor = StubCursor<Certification>(CertCollectionName);
			StubSingleFoundResult(cursor, cert);
			var patch = new JsonPatchDocument<Certification>();

			var response = _controller.Awaiting(x => x.Patch(Id, patch));

			response.Should()
				.ThrowExactly<HttpBadRequestException>()
				.Which.StatusCode
				.Should().Be((int)HttpStatusCode.BadRequest);
		}

		[Test]
		public void should_throw_bad_request_exception_when_activating_cert_with_no_active_courses()
		{
			var cert = new Certification { IsActive = false };
			var cursor = StubCursor<Certification>(CertCollectionName);
			StubSingleFoundResult(cursor, cert);
			var patch = new JsonPatchDocument<Certification>();
			var op = new Operation<Certification>("replace", "isActive", "", true);
			patch.Operations.Add(op);

			_controller.Awaiting(x => x.Patch(Id, patch))
				.Should().ThrowExactly<HttpBadRequestException>()
				.Which.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		[Test]
		public async Task should_increment_version_number_when_activating_cert()
		{
			const int versionNumber = 3;
			var cert = new Certification
			{
				IsActive = false,
				CertificationVersion = versionNumber,
				Courses = new List<DenormalizedCourse> { new DenormalizedCourse { IsActive = true } }
			};
			var cursor = StubCursor<Certification>(CertCollectionName);
			StubSingleFoundResult(cursor, cert);
			var patch = new JsonPatchDocument<Certification>();
			var op = new Operation<Certification>("replace", "isActive", "", true);
			patch.Operations.Add(op);

			var response = await _controller.Patch(Id, patch);

			response.Data.CertificationVersion.Should().Be(versionNumber + 1);
		}

		#endregion

		#region Delete

		[Test]
		public async Task should_delegate_to_repo_to_delete_document_and_related_course_documents()
		{
			var objectId = ObjectId.Parse(Id);
			var certificationCursor = StubCursor<Certification>(CertCollectionName);
			var cert = new Certification { Id = objectId };
			StubSingleFoundResult(certificationCursor, cert);
			Repository.Setup(x => x.DeleteManyAsync(CertCollectionName, It.IsAny<Expression<Func<Certification, bool>>>()))
				.ReturnsAsync(new DeleteResult.Acknowledged(1));
			Repository.Setup(x => x.DeleteAsync(CertCollectionName, It.IsAny<Expression<Func<Certification, bool>>>()))
				.ReturnsAsync(new DeleteResult.Acknowledged(1));

			await ExecuteEndpoint(_controller.Delete(Id));

			Repository.Verify(x => x.DeleteAsync(CertCollectionName,
				It.Is<Expression<Func<Certification, bool>>>(e => Lambda.Eq(e, f => f.Id == objectId))));
			Repository.Verify(x => x.DeleteManyAsync(CourseCollectionName,
				It.Is<Expression<Func<Course, bool>>>(e => Lambda.Eq(e, f => f.CertificationId == Id))));
		}

		#endregion

		#region Private methods

		private void AssertRepositoryWasQueriedById(ObjectId objectId)
		{
			Repository.Verify(x => x.FindAsync(CertCollectionName,
				It.Is<Expression<Func<Certification, bool>>>(e => Lambda.Eq(e, f => f.Id == objectId))));
		}

		#endregion
	}
}
