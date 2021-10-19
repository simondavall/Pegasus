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
    public class CheckRecoveryCodeStatusTests : BaseControllerTest
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
        public async Task CheckRecoveryCodesStatus_ModelNullOrUserIdNullOrUserNotFound_ReturnsModelWithError()
        {
            var sut = CreateManageController();
            await CaseModelIsNull<RecoveryCodeStatusModel, RecoveryCodeStatusModel>(sut.CheckRecoveryCodesStatus);
            await CaseUserIdIsNull<RecoveryCodeStatusModel, RecoveryCodeStatusModel>(sut.CheckRecoveryCodesStatus);
            await CaseUserNotFound<RecoveryCodeStatusModel, RecoveryCodeStatusModel>(sut.CheckRecoveryCodesStatus);
        }
        
        // [Test]
        // public async Task CheckRecoveryCodesStatus_ModelNull_ReturnsModelWithError()
        // {
        //     var sut = CreateManageController();
        //     
        //     var result = await sut.CheckRecoveryCodesStatus(null);
        //
        //     VerifyErrorLogged(Times.Never());
        //     result.Should().BeOfType<RecoveryCodeStatusModel>();
        //     result.IsUpdated.Should().BeFalse();
        //     AssertHasErrors(result, 1);
        // }
        //
        // [Test]
        // public async Task CheckRecoveryCodesStatus_UserIsNull_ReturnsModelWithError()
        // {
        //     var sut = CreateManageController();
        //     var result = await sut.CheckRecoveryCodesStatus(new RecoveryCodeStatusModel {UserId = BadUserId});
        //
        //     VerifyErrorLogged(Times.Once());
        //     result.Should().BeOfType<RecoveryCodeStatusModel>();
        //     result.IsUpdated.Should().BeFalse();
        //     AssertHasErrors(result, 1);
        // }

        [Test]
        public async Task CheckRecoveryCodesStatus_RecoveryCodesExist_ReturnsModelIsUpdatedFalse()
        {
            const int anyNumberGreaterThanZero = 5;
            MockUserManager.Setup(x => x.CountRecoveryCodesAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(anyNumberGreaterThanZero);
            
            var sut = CreateManageController();
            var result = await sut.CheckRecoveryCodesStatus(new RecoveryCodeStatusModel {UserId = UserId});

            VerifyErrorLogged(Times.Never());
            result.Should().BeOfType<RecoveryCodeStatusModel>();
            result.IsUpdated.Should().BeFalse();
            AssertHasErrors(result, 0);
        }
        
        [Test]
        public async Task CheckRecoveryCodesStatus_GenerateFails_ReturnsModelIsUpdatedFalse()
        {
            const int noRecoveryCodesLeft = 0;
            MockUserManager.Setup(x => x.CountRecoveryCodesAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(noRecoveryCodesLeft);
            MockUserManager.Setup(x =>
                    x.GenerateNewTwoFactorRecoveryCodesAsync(It.IsAny<IdentityUser>(), It.IsAny<int>()))
                .ReturnsAsync(new List<string>());
            
            var sut = CreateManageController();
            var result = await sut.CheckRecoveryCodesStatus(new RecoveryCodeStatusModel {UserId = UserId});

            VerifyErrorLogged(Times.Once());
            result.Should().BeOfType<RecoveryCodeStatusModel>();
            result.RecoveryCodes.Should().BeEmpty();
            result.IsUpdated.Should().BeFalse();
            AssertHasErrors(result, 1);
        }
        [Test]
        public async Task CheckRecoveryCodesStatus_NoRecoveryCodesExist_ReturnsModelIsUpdatedTrue()
        {
            const int noRecoveryCodesLeft = 0;
            MockUserManager.Setup(x => x.CountRecoveryCodesAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(noRecoveryCodesLeft);
            MockUserManager.Setup(x =>
                    x.GenerateNewTwoFactorRecoveryCodesAsync(It.IsAny<IdentityUser>(), It.IsAny<int>()))
                .ReturnsAsync(new List<string> {"code1", "code2"});
            
            var sut = CreateManageController();
            var result = await sut.CheckRecoveryCodesStatus(new RecoveryCodeStatusModel {UserId = UserId});

            VerifyErrorLogged(Times.Never());
            result.Should().BeOfType<RecoveryCodeStatusModel>();
            result.RecoveryCodes.Should().NotBeEmpty();
            result.IsUpdated.Should().BeTrue();
            AssertHasErrors(result, 0);
        }
    }
}