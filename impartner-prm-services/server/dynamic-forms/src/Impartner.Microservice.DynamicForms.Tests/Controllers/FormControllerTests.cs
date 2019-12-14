using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Impartner.Microservice.Common.Exceptions.Http;
using Impartner.Microservice.Common.Models.Responses;
using Impartner.Microservice.Common.Mongo.Repositories;
using Impartner.Microservice.DynamicForms.Controllers;
using Impartner.Microservice.DynamicForms.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using Neleus.LambdaCompare;
using NUnit.Framework;
using DynamicForm = Impartner.Microservice.DynamicForms.Models.DynamicForm.V1;

namespace Impartner.Microservice.DynamicForms.Tests.Controllers
{
	[TestFixture]
	public class FormControllerTests
	{
		private Mock<IMongoRepository> _repository;
		private FormController _controller;
		private List<DynamicForm> _data;
		private Mock<IFindFluent<DynamicForm, DynamicForm>> _findCursor;
		private Mock<IAsyncCursor<DynamicForm>> _asyncCursor;
		private const string Id = "5bd7882115a42375281e10c1";

		[SetUp]
		public void SetUp()
		{
			_repository = new Mock<IMongoRepository>();
			_controller = new FormController(_repository.Object)
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = new DefaultHttpContext()
				},
				ObjectValidator = Mock.Of<IObjectModelValidator>()
			};
			StubCursors();
		}

		private void StubCursors()
		{
			_data = new List<DynamicForm>();
			for (var i = 0; i < 5; i++)
				_data.Add(new DynamicForm { ObjectName = "test" + i });
			_findCursor = new Mock<IFindFluent<DynamicForm, DynamicForm>>();
			_findCursor.Setup(x => x.Skip(It.IsAny<int>())).Returns(_findCursor.Object);
			_findCursor.Setup(x => x.Limit(It.IsAny<int>())).Returns(_findCursor.Object);
			_asyncCursor = new Mock<IAsyncCursor<DynamicForm>>();
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
		
			_repository.Setup(x => x.Find(FormController.CollectionName, It.IsAny<FilterDefinition<DynamicForm>>())).Returns(_findCursor.Object);
		}

		private static void StubSingleFoundResult(Mock<IAsyncCursor<DynamicForm>> cursor, DynamicForm form)
		{
			cursor.Setup(x => x.Current).Returns(new List<DynamicForm> { form });
			cursor.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>()))
				.Returns(true)
				.Returns(false);
		}

		private static void StubNotFoundResult(Mock<IAsyncCursor<DynamicForm>> cursor)
		{
			cursor.Setup(x => x.Current).Returns(new List<DynamicForm>());
			cursor.Setup(x => x.MoveNext(It.IsAny<CancellationToken>()))
				.Returns(false);
		}

		private static async Task<TData> ExecuteEndpoint<TData>(Task<ApiResult<TData>> executeEndpoint)
		{
			var actionResult = await executeEndpoint;

			return actionResult.Data;
		} 

		#region Get

		[Test]
		public async Task should_call_find_on_collection_returning_all_results()
		{
			var body = await ExecuteEndpoint(_controller.Get());

			_repository.Verify(x => x.Find(FormController.CollectionName, It.IsAny<FilterDefinition<DynamicForm>>()));
			body.Should().NotBeNull();
			body.Should().HaveCount(25);
			_findCursor.Verify(x => x.Skip(0));
			_findCursor.Verify(x => x.Limit(100));
		}

		[Test]
		public async Task should_add_skip_and_limit_when_specified()
		{
			const int numberOfResults = 5;
			const int skip = 20;

			var _ = await _controller.Get(take:numberOfResults, skip:skip);

			_findCursor.Verify(x => x.Skip(skip));
			_findCursor.Verify(x => x.Limit(numberOfResults));
		}

		#endregion
		
		#region Get(string id)

		[Test]
		public async Task should_delegate_to_find_results_with_expected_params()
		{
			var objectId = ObjectId.Parse(Id);
			var cursor = StubCursor();
			var form = new DynamicForm();
			StubSingleFoundResult(cursor, form);

			var body = await ExecuteEndpoint(_controller.GetById(Id));

			AssetRepositoryWasQueriedById(objectId);
			body.Should().Be(form);
		}

		private void AssetRepositoryWasQueriedById(ObjectId objectId)
		{
			_repository.Verify(x => x.FindAsync(FormController.CollectionName,
				It.Is<Expression<Func<DynamicForm, bool>>>(e => Lambda.Eq(e, f => f.Id == objectId))));
		}

		private Mock<IAsyncCursor<DynamicForm>> StubCursor()
		{
			var cursor = new Mock<IAsyncCursor<DynamicForm>>();
			_repository.Setup(x => x.FindAsync(FormController.CollectionName,
					It.IsAny<Expression<Func<DynamicForm, bool>>>()))
				.ReturnsAsync(cursor.Object);
			return cursor;
		}

		[Test]
		public void should_return_not_found_404_when_no_result_in_database()
		{
			var cursor = StubCursor();
			StubNotFoundResult(cursor);

			_controller.Awaiting(x => x.GetById(Id))
				.Should().ThrowExactly<HttpNotFoundException>().And
				.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Test]
		public void should_return_bad_request_with_bad_id()
		{
			_controller.Awaiting(x => x.GetById("Invalid object id"))
				.Should().ThrowExactly<HttpBadRequestException>().And
				.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		#endregion

		#region Upsert

		[Test]
		public void should_return_not_found_when_no_form_found_by_object_id()
		{
			var model = new DynamicFormCopy{Id = ObjectId.GenerateNewId().ToString()};
			var cursor = StubCursor();
			StubNotFoundResult(cursor);

			_controller.Awaiting(x => x.Upsert(model))
				.Should().ThrowExactly<HttpNotFoundException>().And
				.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}
		
		[Test]
		public async Task should_return_existing_draft_copy()
		{
			var id = ObjectId.GenerateNewId();
			var model = new DynamicFormCopy{Id = id.ToString()};
			var cursor = StubCursor();
			var form = new DynamicForm{Id = id};
			cursor.Setup(x => x.Current).Returns(new List<DynamicForm> { form });
			cursor.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>()))
				.Returns(true)
				.Returns(true)
				.Returns(false);
			
			var body = await ExecuteEndpoint(_controller.Upsert(model));

			_repository.Verify(x => x.SaveAsync(
				FormController.CollectionName,
				It.IsAny<DynamicForm>()), Times.Never);
			body.Should().Be(form);
		}		
		
		[Test]
		public async Task should_save_new_copy_of_original_form()
		{
			var id = ObjectId.GenerateNewId();
			var model = new DynamicFormCopy{Id = id.ToString()};
			var cursor = StubCursor();
			var form = new DynamicForm{Id = id};
			StubSingleFoundResult(cursor, form);
			_repository.Setup(x => x.SaveAsync
				(
					FormController.CollectionName, It.IsAny<DynamicForm>()
				)
			)
			.ReturnsAsync(form);
			
			var _ = await _controller.Upsert(model);

			_repository.Verify(x => x.SaveAsync(
				FormController.CollectionName,
				It.IsAny<DynamicForm>()));
		}

		#endregion

		#region AddNewItem

		[Test]
		public async Task should_delegate_to_repo_to_save_form()
		{
			var form = new DynamicForm();
			_repository.Setup(x => x.SaveAsync(
				FormController.CollectionName,
				form)).ReturnsAsync(form);

			var body = await ExecuteEndpoint(_controller.AddNewItem(form));

			_repository.Verify(x => x.SaveAsync(
				FormController.CollectionName,
				form));
			body.Should().Be(form);
		}

		#endregion

		#region Patch

		[Test]
		public async Task should_delegate_to_repo_to_lookup_by_id()
		{
			var objectId = ObjectId.Parse(Id);
			var cursor = StubCursor();
			var form = new DynamicForm();
			StubSingleFoundResult(cursor, form);
			var patch = new JsonPatchDocument<DynamicForm>();
			_repository.Setup(x => x.UpdateAsync(FormController.CollectionName, It.IsAny<Expression<Func<DynamicForm, bool>>>(), form))
				.ReturnsAsync(new ReplaceOneResult.Acknowledged(1, 1, objectId));

			var _ = await _controller.Patch(Id, patch);

			AssetRepositoryWasQueriedById(objectId);
		}

		[Test]
		public void should_return_not_found_result_when_no_object_found_by_id()
		{
			var cursor = StubCursor();
			StubNotFoundResult(cursor);
			var patch = new JsonPatchDocument<DynamicForm>();

			_controller.Awaiting(x => x.Patch(Id, patch))
				.Should().ThrowExactly<HttpNotFoundException>().And
				.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Test]
		public async Task should_apply_patch_then_delegate_to_repo_to_update_form_data()
		{
			var objectId = ObjectId.Parse(Id);
			var cursor = StubCursor();
			var form = new DynamicForm();
			StubSingleFoundResult(cursor, form);
			var patch = new JsonPatchDocument<DynamicForm>();
			_repository.Setup(x => x.UpdateAsync(FormController.CollectionName, It.IsAny<Expression<Func<DynamicForm, bool>>>(), form))
				.ReturnsAsync(new ReplaceOneResult.Acknowledged(1, 1, objectId));

			var _ = await _controller.Patch(Id, patch);

			_repository.Verify(x => x.UpdateAsync(FormController.CollectionName,
				It.Is<Expression<Func<DynamicForm, bool>>>(e => Lambda.Eq(e, f => f.Id == objectId)), form));
		}

		#endregion

		#region Delete

		[Test]
		public async Task should_delegate_to_repo_to_delete_document()
		{
			_repository.Setup(x => x.DeleteAsync(FormController.CollectionName, It.IsAny<Expression<Func<DynamicForm, bool>>>()))
				.ReturnsAsync(new DeleteResult.Acknowledged(1));
			var objectId = ObjectId.Parse(Id);

			var _ = await _controller.Delete(Id);

			_repository.Verify(x => x.DeleteAsync(FormController.CollectionName,
				It.Is<Expression<Func<DynamicForm, bool>>>(e => Lambda.Eq(e, f => f.Id == objectId))));
		}

		#endregion
	}
}
