using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Impartner.Microservice.Common.Models.Responses;
using Impartner.Microservice.Common.Mongo.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using ClaimTypes = Impartner.Common.Security.Constants.ClaimTypes;
using TenantDocument = Impartner.Microservice.Common.Mongo.Models.TenantDocument.V1;

namespace Impartner.Microservice.WidgetRegistration.Tests
{
	public abstract class ContextBaseTests
	{
		protected Mock<IMongoRepository> Repository;
		protected const int TenantId = 40;
		protected const string UserId = "14";
		protected const string Username = "username@username.co";
		protected const string FirstName = "Firsty";
		protected const string LastName = "Lasty";
		protected const string Id = "5bd7882115a42375281e10c1";

		[SetUp]
		public void BaseSetUp()
		{
			Repository = new Mock<IMongoRepository>();
		}

		protected void StubRequest(ControllerBase controller)
		{
			controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
			controller.ControllerContext.HttpContext.User = GenerateClaimsPrincipal();
			controller.ObjectValidator = Mock.Of<IObjectModelValidator>();
		}

		protected ClaimsPrincipal GetClaimsPrincipal() => GenerateClaimsPrincipal();

		protected ClaimsPrincipal GetInvalidClaimsPrincipal() => GenerateClaimsPrincipal(false);

		private ClaimsPrincipal GenerateClaimsPrincipal(bool shouldBeValid = true)
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.TenantId, TenantId.ToString()),
				new Claim(ClaimTypes.UserId, UserId),
				new Claim(System.Security.Claims.ClaimTypes.NameIdentifier, Username),
				new Claim(System.Security.Claims.ClaimTypes.GivenName, FirstName),
			};

			if (shouldBeValid)
				claims.Add(new Claim(System.Security.Claims.ClaimTypes.Surname, LastName));

			return new ClaimsPrincipal(new ClaimsIdentity(claims));
		}

		protected static void StubNotFoundResult<T>(Mock<IAsyncCursor<T>> cursor)
		{
			cursor.Setup(x => x.Current).Returns(new List<T>());
			cursor.Setup(x => x.MoveNext(It.IsAny<CancellationToken>()))
				.Returns(false);
		}

		protected static void StubSingleFoundResult<T>(Mock<IAsyncCursor<T>> cursor, T form)
		{
			cursor.Setup(x => x.Current).Returns(new List<T> { form });
			cursor.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>()))
				.Returns(true)
				.Returns(false);
		}

		protected Mock<IAsyncCursor<T>> StubCursor<T>(string collectionName) where T : TenantDocument
		{
			var cursor = new Mock<IAsyncCursor<T>>();
			Repository.Setup(x => x.FindAsync(collectionName, It.IsAny<Expression<Func<T, bool>>>()))
				.ReturnsAsync(cursor.Object);
			return cursor;
		}

		protected static async Task<TData> ExecuteEndpoint<TData>(Task<ApiResult<TData>> executeEndpoint)
		{
			var actionResult = await executeEndpoint;

			return actionResult.Data;
		}

		protected static void StubListOfResult<T>(Mock<IAsyncCursor<T>> cursor, IEnumerable<T> data)
		{
			cursor.Setup(x => x.Current).Returns(data);
			cursor.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>()))
				.Returns(true)
				.Returns(false);
		}
	}
}
