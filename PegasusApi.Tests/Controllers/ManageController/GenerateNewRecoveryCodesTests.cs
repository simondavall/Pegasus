using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using PegasusApi.Library.DataAccess;
using PegasusApi.Library.Models.Manage;

namespace PegasusApi.Tests.Controllers.ManageController
{
    public class GenerateNewRecoveryCodesTests : BaseControllerTest
    {
        private Mock<IUsersData> _mockUsersData;

        [SetUp]
        public void EachTestSetup()
        {
            _mockUsersData = new Mock<IUsersData>();
        }

        private PegasusApi.Controllers.ManageController CreateManageController()
        {
            return new PegasusApi.Controllers.ManageController(MockUserManager.Object, _mockUsersData.Object, null, null, MockLogger.Object);
        }
        
        [Test]
        public async Task GenerateNewRecoveryCodes_UserIdIsNullOrNotFound_ReturnsModelWithError()
        {
            var sut = CreateManageController();
            await CaseUserIdIsNull(sut.GenerateNewRecoveryCodes);
            await CaseUserNotFound(sut.GenerateNewRecoveryCodes);
        }

        [Test]
        public async Task GenerateNewRecoveryCodes_GenerateFailed_ReturnsModelWithError()
        {
            MockUserManager.Setup(x =>
                    x.GenerateNewTwoFactorRecoveryCodesAsync(It.IsAny<IdentityUser>(), It.IsAny<int>()))
                .ReturnsAsync(new List<string>());
            
            var sut = CreateManageController();
            var result = await sut.GenerateNewRecoveryCodes(UserId);

            VerifyErrorLogged(Times.Once());
            result.Should().BeOfType<GenerateRecoveryCodesModel>();
            result.RecoveryCodes.Should().BeEmpty();
            AssertHasErrors(result, 1);
        }
        
        [Test]
        public async Task GenerateNewRecoveryCodes_GenerateSuccess_ReturnsModelWithCodes()
        {
            MockUserManager.Setup(x =>
                    x.GenerateNewTwoFactorRecoveryCodesAsync(It.IsAny<IdentityUser>(), It.IsAny<int>()))
                .ReturnsAsync(new List<string> {"code1", "code2"});
            
            var sut = CreateManageController();
            var result = await sut.GenerateNewRecoveryCodes(UserId);

            VerifyErrorLogged(Times.Never());
            result.Should().BeOfType<GenerateRecoveryCodesModel>();
            result.RecoveryCodes.Should().NotBeEmpty();
            AssertHasErrors(result, 0);
        }
    }
}