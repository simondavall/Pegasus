using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using PegasusApi.Models.Account;

namespace PegasusApi.Tests.Controllers.AccountController
{
    public class RememberClientTests : AccountControllerTestsBase
    {
        
        [Test]
        public async Task RememberClient_UserIsNull_ReturnsModel()
        {
            var sut = CreateAccountController();
            var result = await sut.RememberClient(new RememberClientModel());

            result.Should().BeOfType<RememberClientModel>();
            result.SecurityStamp.Should().BeNull();
        }

        [Test]
        public async Task RememberClient_UserWithSecurityStamp_ReturnsModelWithSecurityStamp()
        {
            var sut = CreateAccountController();
            var result = await sut.RememberClient(new RememberClientModel { UserId = UserId });

            result.Should().BeOfType<RememberClientModel>();
            result.SecurityStamp.Should().Be("security-stamp");
        }

    }
}