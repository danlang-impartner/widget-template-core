using FluentAssertions;
using Impartner.Microservice.Common.Extensions;
using NUnit.Framework;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace Impartner.Microservice.Training.Tests.Extensions
{
	[TestFixture]
	public class ClaimsPrincipalExtensionTests : ContextBaseTests
	{
		[Test]
		public void should_generate_user_object_from_claims_principal()
		{
			var claimsPrincipal = GetClaimsPrincipal();

			var user = claimsPrincipal.ToUserInfo();

			user.UserId.Should().Be(UserId);
			user.Username.Should().Be(Username);
			user.FirstName.Should().Be(FirstName);
			user.LastName.Should().Be(LastName);
		}

		[Test]
		public void should_throw_authentication_exception_when_an_expected_claim_is_missing()
		{
			var claimsPrincipal = GetInvalidClaimsPrincipal();

			claimsPrincipal.Invoking(cp => cp.ToUserInfo()).Should().Throw<AuthenticationException>();
		}
	}
}
