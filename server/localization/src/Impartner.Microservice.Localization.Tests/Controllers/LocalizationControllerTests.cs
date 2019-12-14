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
using Impartner.Microservice.Localization.Controllers;
using Impartner.Microservice.Localization.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using Neleus.LambdaCompare;
using NUnit.Framework;

using LocalizationModel = Impartner.Microservice.Localization.Models.LocalizationModel.V1;

namespace Impartner.Microservice.Localization.Tests.Controllers
{
	[TestFixture]
	public class LocalizationControllerTests
	{
		private Mock<IMongoRepository> _repository;
		private LocalizationController _controller;
		private List<LocalizationModel> _data;
		private Mock<IFindFluent<LocalizationModel, LocalizationModel>> _findCursor;
		private Mock<IAsyncCursor<LocalizationModel>> _asyncCursor;
		private const string Id = "5bd7882115a42375281e10c1";
		private const string ObjectName = "Account";
		
		[SetUp]
		public void SetUp()
		{
			_repository = new Mock<IMongoRepository>();
			_controller = new LocalizationController(_repository.Object)
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
			_data = new List<LocalizationModel>();
			for (var i = 0; i < 5; i++)
				_data.Add(new LocalizationModel { ObjectName = "test" + i });
			_findCursor = new Mock<IFindFluent<LocalizationModel, LocalizationModel>>();
			_findCursor.Setup(x => x.Skip(It.IsAny<int>())).Returns(_findCursor.Object);
			_findCursor.Setup(x => x.Limit(It.IsAny<int>())).Returns(_findCursor.Object);
			_asyncCursor = new Mock<IAsyncCursor<LocalizationModel>>();
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
		
			_repository.Setup(x => x.Find(LocalizationController.CollectionName, It.IsAny<FilterDefinition<LocalizationModel>>())).Returns(_findCursor.Object);
		}

		private static void StubSingleFoundResult(Mock<IAsyncCursor<LocalizationModel>> cursor, LocalizationModel model)
		{
			cursor.Setup(x => x.Current).Returns(new List<LocalizationModel> { model });
			cursor.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>()))
				.Returns(true)
				.Returns(false);
		}

		private static void StubNotFoundResult(Mock<IAsyncCursor<LocalizationModel>> cursor)
		{
			cursor.Setup(x => x.Current).Returns(new List<LocalizationModel>());
			cursor.Setup(x => x.MoveNext(It.IsAny<CancellationToken>()))
				.Returns(false);
		}
		
		private static void AssetNotFoundResultWasReturned(ActionResult<LocalizationModel> response)
		{
			response.Value.Should().BeNull();
			response.Result.Should()
				.BeOfType<NotFoundResult>()
				.Which.StatusCode
				.Should().Be((int)HttpStatusCode.NotFound);
		}

		private static async Task<TData> ExecuteEndpoint<TData>(Task<ApiResult<TData>> executeEndpoint)
		{
			var actionResult = await executeEndpoint;

			return actionResult.Data;
		} 

		#region Get

		[Test]
		public void should_call_find_on_collection_returning_no_results_when_not_found()
		{
			const string objectId = "123";
			const string objectName = "form";
			var cursor = new Mock<IAsyncCursor<LocalizationModel>>();
			StubFindAsync(cursor.Object);

			_controller.Awaiting(x => x.Get(objectId, objectName))
				.Should().ThrowExactly<HttpNotFoundException>().And
				.StatusCode.Should().Be(HttpStatusCode.NotFound);

			_repository.Verify(x => x.FindAsync(LocalizationController.CollectionName, 
				It.Is<Expression<Func<LocalizationModel, bool>>>(e => Lambda.Eq(e, f => f.ObjectId == objectId && f.ObjectName == objectName))));
		}

		private void StubFindAsync(IAsyncCursor<LocalizationModel> cursorObject)
		{
			_repository.Setup(x => x.FindAsync(LocalizationController.CollectionName,
				It.IsAny<Expression<Func<LocalizationModel, bool>>>())).ReturnsAsync(cursorObject);
		}

		[Test]
		public void should_throw_exception_if_more_than_one_result_returned()
		{
			const string objectId = "123";
			const string objectName = "form";
			StubFindAsync(_asyncCursor.Object);

			_controller.Awaiting(x => x.Get(objectId, objectName)).Should().Throw<InvalidOperationException>();
		}

		[Test]
		public async Task should_return_single_result_when_found()
		{
			const string objectId = "123";
			const string objectName = "form";
			var model = new LocalizationModel();
			var cursor = new Mock<IAsyncCursor<LocalizationModel>>();
			cursor.Setup(x => x.Current).Returns(new List<LocalizationModel> {model});
			cursor.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);
			StubFindAsync(cursor.Object);

			var response = await ExecuteEndpoint(_controller.Get(objectId, objectName));

			response.Should().Be(model);
		}

		#endregion

		#region Get(string id)

		[Test]
		public async Task should_delegate_to_find_results_with_expected_params()
		{
			var objectId = ObjectId.Parse(Id);
			var cursor = StubCursor();
			var model = new LocalizationModel();
			StubSingleFoundResult(cursor, model);

			var response = await ExecuteEndpoint(_controller.GetById(Id));

			AssetRespositoryWasQueriedById(objectId);
			response.Should().Be(model);
		}

		private void AssetRespositoryWasQueriedById(ObjectId objectId)
		{
			_repository.Verify(x => x.FindAsync(LocalizationController.CollectionName,
				It.Is<Expression<Func<LocalizationModel, bool>>>(e => Lambda.Eq(e, f => f.Id == objectId))));
		}

		private Mock<IAsyncCursor<LocalizationModel>> StubCursor()
		{
			var cursor = new Mock<IAsyncCursor<LocalizationModel>>();
			_repository.Setup(x => x.FindAsync(LocalizationController.CollectionName,
					It.IsAny<Expression<Func<LocalizationModel, bool>>>()))
				.ReturnsAsync(cursor.Object);
			return cursor;
		}

		[Test]
		public void should_return_not_found_404_when_no_result_in_dbase()
		{
			var cursor = StubCursor();
			StubNotFoundResult(cursor);

			_controller.Awaiting(x => x.GetById(Id))
				.Should().ThrowExactly<HttpNotFoundException>().And
				.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		#endregion

		#region AddNewItem

		[Test]
		public async Task should_delegate_to_repo_to_look_for_duplicate_object()
		{
			var cursor = new Mock<IAsyncCursor<LocalizationModel>>();
			StubFindAsync(cursor.Object);
			var model = new LocalizationModel();
			_repository.Setup(x => x.SaveAsync(
				LocalizationController.CollectionName,
				model)).ReturnsAsync(model);

			await _controller.AddNewItem(model);

			_repository.Verify(x => x.FindAsync(LocalizationController.CollectionName, 
				It.Is<Expression<Func<LocalizationModel, bool>>>(e => Lambda.Eq(e, f => f.ObjectId == model.ObjectId && f.ObjectName == model.ObjectName))));

		}
		
		[Test]
		public void should_return_403_when_duplicate_item_already_exists()
		{
			var model = new LocalizationModel();
			var cursor = new Mock<IAsyncCursor<LocalizationModel>>();
			cursor.Setup(x => x.Current).Returns(new List<LocalizationModel> {model});
			cursor.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);
			StubFindAsync(cursor.Object);

			_controller.Awaiting(x => x.AddNewItem(model))
				.Should().ThrowExactly<HttpForbidException>().And
				.StatusCode.Should().Be(HttpStatusCode.Forbidden);
		}		
		
		[Test]
		public async Task should_delegate_to_repo_to_save_model_when_it_doesnt_already_exist()
		{
			var cursor = new Mock<IAsyncCursor<LocalizationModel>>();
			StubFindAsync(cursor.Object);
			
			var model = new LocalizationModel();
			_repository.Setup(x => x.SaveAsync(
				LocalizationController.CollectionName,
				model)).ReturnsAsync(model);

			var response = await ExecuteEndpoint(_controller.AddNewItem(model));

			_repository.Verify(x => x.SaveAsync(
				LocalizationController.CollectionName,
				model));
			response.Should().Be(model);
		}

		#endregion
		
		#region Upsert

		[Test]
		public void should_return_not_found_when_no_form_found_by_object_id()
		{
			var model = new LocalizationCopy
			{
				ObjectId = ObjectId.GenerateNewId().ToString()
			};
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
			var model = new LocalizationCopy
			{
				ObjectId = ObjectId.GenerateNewId().ToString(),
				ObjectName = ObjectName
			};
			var cursor = StubCursor();
			var form = new LocalizationModel{Id = id};
			cursor.Setup(x => x.Current).Returns(new List<LocalizationModel> { form });
			cursor.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>()))
				.Returns(true)
				.Returns(true)
				.Returns(false);
			
			var response = await ExecuteEndpoint(_controller.Upsert(model));

			_repository.Verify(x => x.SaveAsync(
				LocalizationController.CollectionName,
				It.IsAny<LocalizationModel>()), Times.Never);
			response.Should().Be(form);
		}		
		
		[Test]
		public async Task should_save_new_copy_of_original_form()
		{
			var id = ObjectId.GenerateNewId();
			var model = new LocalizationCopy
			{
				ObjectId = ObjectId.GenerateNewId().ToString()
			};
			var cursor = StubCursor();
			var localizationModel = new LocalizationModel{Id = id};
			cursor.Setup(x => x.Current).Returns(new List<LocalizationModel> { localizationModel });
			cursor.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>()))
				.Returns(false)
				.Returns(true)
				.Returns(false);
			_repository.Setup(x => x.SaveAsync
				(
					LocalizationController.CollectionName, It.IsAny<LocalizationModel>()
				)
			)
			.ReturnsAsync(localizationModel);
			
			var _ = await _controller.Upsert(model);

			_repository.Verify(x => x.SaveAsync(
				LocalizationController.CollectionName,
				It.IsAny<LocalizationModel>()));
		}

		#endregion

		#region Patch

		[Test]
		public async Task should_delegate_to_repo_to_lookup_by_id()
		{
			var objectId = ObjectId.Parse(Id);
			var cursor = StubCursor();
			var model = new LocalizationModel();
			StubSingleFoundResult(cursor, model);
			var patch = new JsonPatchDocument<LocalizationModel>();
			_repository.Setup(x => x.UpdateAsync(LocalizationController.CollectionName, It.IsAny<Expression<Func<LocalizationModel, bool>>>(), model))
				.ReturnsAsync(new ReplaceOneResult.Acknowledged(1, 1, objectId));

			var response = await _controller.Patch(Id, patch);

			AssetRespositoryWasQueriedById(objectId);
		}

		[Test]
		public void should_return_not_found_result_when_no_object_found_by_id()
		{
			var cursor = StubCursor();
			StubNotFoundResult(cursor);
			var patch = new JsonPatchDocument<LocalizationModel>();

			_controller.Awaiting(x => x.Patch(Id, patch))
				.Should().ThrowExactly<HttpNotFoundException>().And
				.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Test]
		public async Task should_apply_patch_then_delegate_to_repo_to_update_model_data()
		{
			var objectId = ObjectId.Parse(Id);
			var cursor = StubCursor();
			var model = new LocalizationModel();
			StubSingleFoundResult(cursor, model);
			var patch = new JsonPatchDocument<LocalizationModel>();
			_repository.Setup(x => x.UpdateAsync(LocalizationController.CollectionName, It.IsAny<Expression<Func<LocalizationModel, bool>>>(), model))
				.ReturnsAsync(new ReplaceOneResult.Acknowledged(1, 1, objectId));

			var response = await _controller.Patch(Id, patch);

			_repository.Verify(x => x.UpdateAsync(LocalizationController.CollectionName,
				It.Is<Expression<Func<LocalizationModel, bool>>>(e => Lambda.Eq(e, f => f.Id == objectId)), model));
		}

		#endregion

		#region Delete

		[Test]
		public async Task should_delegate_to_repo_to_delete_document()
		{
			_repository.Setup(x => x.DeleteAsync(LocalizationController.CollectionName, It.IsAny<Expression<Func<LocalizationModel, bool>>>()))
				.ReturnsAsync(new DeleteResult.Acknowledged(1));
			var objectId = ObjectId.Parse(Id);

			var response = await _controller.Delete(Id);

			_repository.Verify(x => x.DeleteAsync(LocalizationController.CollectionName,
				It.Is<Expression<Func<LocalizationModel, bool>>>(e => Lambda.Eq(e, f => f.Id == objectId))));
		}

		#endregion
	}
}
