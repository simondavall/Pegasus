using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Pegasus.Library.Api;
using Pegasus.Library.Models.Manage;
using Pegasus.Services;
using Pegasus.Library.Services.Resources;

namespace PegasusTests.Controllers.ManageController
{
    class Disable2FaTests : ManageControllerTestsBase
    {
        [Test]
        public async Task GET_Disable2Fa_IsEnabled_ReturnsViewResult()
        {
            _mockApiHelper.Setup(x => x.GetFromUri<GetTwoFactorEnabledModel>(It.IsAny<string>()))
                .ReturnsAsync(new GetTwoFactorEnabledModel { IsEnabled = true});
            var manageEndpoint = new ManageEndpoint(_mockApiHelper.Object);

            var signInManager = new SignInManager(_mockHttpContextWrapper.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object,_mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);

            var sut = new Pegasus.Controllers.ManageController(manageEndpoint, signInManager, _logger.Object)
            {
                ControllerContext = _controllerContext
            };

            var result = await sut.Disable2Fa();

            _mockApiHelper.Verify(x => x.GetFromUri<GetTwoFactorEnabledModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.Zero(sut.ModelState.ErrorCount,  "Failed error count");
        }

        [Test]
        public async Task GET_Disable2Fa_IsNotEnabled_ReturnsViewResultWithMessage()
        {
            _mockApiHelper.Setup(x => x.GetFromUri<GetTwoFactorEnabledModel>(It.IsAny<string>()))
                .ReturnsAsync(new GetTwoFactorEnabledModel { IsEnabled = false});
            var manageEndpoint = new ManageEndpoint(_mockApiHelper.Object);

            var signInManager = new SignInManager(_mockHttpContextWrapper.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object,_mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);

            var sut = new Pegasus.Controllers.ManageController(manageEndpoint, signInManager, _logger.Object)
            {
                ControllerContext = _controllerContext
            };

            var result = await sut.Disable2Fa();

            _mockApiHelper.Verify(x => x.GetFromUri<GetTwoFactorEnabledModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual(Resources.ControllerStrings.ManageController.CannotDisable2Fa, ((Disable2FaModel)((ViewResult)result).Model).StatusMessage);
        }

        [Test]
        public async Task POST_Disable2Fa_IsNotEnabled_ReturnsViewResultWithMessage()
        {
            _mockApiHelper.Setup(x => x.GetFromUri<GetTwoFactorEnabledModel>(It.IsAny<string>()))
                .ReturnsAsync(new GetTwoFactorEnabledModel { IsEnabled = false});
            _mockApiHelper.Setup(x => x.PostAsync(It.IsAny<SetTwoFactorEnabledModel>(), It.IsAny<string>()))
                .ReturnsAsync(new SetTwoFactorEnabledModel {Succeeded = false, Errors = new List<IdentityError> {new IdentityError {Description = "Error Message"}}, StatusMessage = "Error"});
            var manageEndpoint = new ManageEndpoint(_mockApiHelper.Object);

            var signInManager = new SignInManager(_mockHttpContextWrapper.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object,_mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);

            var sut = new Pegasus.Controllers.ManageController(manageEndpoint, signInManager, _logger.Object)
            {
                ControllerContext = _controllerContext
            };

            var result = await sut.Disable2Fa(new Disable2FaModel());

            _mockApiHelper.Verify(x => x.GetFromUri<GetTwoFactorEnabledModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual(Resources.ControllerStrings.ManageController.CannotDisable2Fa, ((Disable2FaModel)((ViewResult)result).Model).StatusMessage);
        }

        [Test]
        public async Task POST_Disable2Fa_NotSucceeded_ReturnsViewResultWithMessage()
        {
            _mockApiHelper.Setup(x => x.GetFromUri<GetTwoFactorEnabledModel>(It.IsAny<string>()))
                .ReturnsAsync(new GetTwoFactorEnabledModel { IsEnabled = true});
            _mockApiHelper.Setup(x => x.PostAsync(It.IsAny<SetTwoFactorEnabledModel>(), It.IsAny<string>()))
                .ReturnsAsync(new SetTwoFactorEnabledModel {Succeeded = false, Errors = new List<IdentityError> {new IdentityError {Description = "Error Message"}}, StatusMessage = "Error"});
            var manageEndpoint = new ManageEndpoint(_mockApiHelper.Object);

            var signInManager = new SignInManager(_mockHttpContextWrapper.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object,_mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);

            var sut = new Pegasus.Controllers.ManageController(manageEndpoint, signInManager, _logger.Object)
            {
                ControllerContext = _controllerContext
            };

            var result = await sut.Disable2Fa(new Disable2FaModel());

            _mockApiHelper.Verify(x => x.PostAsync(It.IsAny<SetTwoFactorEnabledModel>(), It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<Disable2FaModel>(((ViewResult) result).Model);
            var taskResult = (Disable2FaModel)((ViewResult)result).Model;
            Assert.AreEqual(Resources.ControllerStrings.ManageController.FailedToDisable2Fa, taskResult.StatusMessage);
        }

        [Test]
        public async Task POST_Disable2Fa_NSucceeded_ReturnsRedirectToAction()
        {
            _mockApiHelper.Setup(x => x.GetFromUri<GetTwoFactorEnabledModel>(It.IsAny<string>()))
                .ReturnsAsync(new GetTwoFactorEnabledModel { IsEnabled = true});
            _mockApiHelper.Setup(x => x.PostAsync(It.IsAny<SetTwoFactorEnabledModel>(), It.IsAny<string>()))
                .ReturnsAsync(new SetTwoFactorEnabledModel {Succeeded = true, StatusMessage = "OK"});
            var manageEndpoint = new ManageEndpoint(_mockApiHelper.Object);

            var signInManager = new SignInManager(_mockHttpContextWrapper.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object,_mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);

            var sut = new Pegasus.Controllers.ManageController(manageEndpoint, signInManager, _logger.Object)
            {
                ControllerContext = _controllerContext
            };

            var result = await sut.Disable2Fa(new Disable2FaModel());

            _mockApiHelper.Verify(x => x.PostAsync(It.IsAny<SetTwoFactorEnabledModel>(), It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.Zero(sut.ModelState.ErrorCount,"Failed error count");
        }
    }
}
