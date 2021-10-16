using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Pegasus.Library.Models.Manage;
using Pegasus.Library.Services.Resources;

namespace Pegasus.Integration.Tests.Controllers.ManageControllerTests
{
    class IndexTests : ManageControllerTestsBase
    {
        [Test]
        public async Task GET_Index_HasErrors_ReturnsViewResultWithErrorMessage()
        {
            MockApiHelper.Setup(x => x.GetFromUri<UserDetailsModel>(It.IsAny<string>()))
                .ReturnsAsync(new UserDetailsModel {Errors = TestErrors, StatusMessage = "Error"});

            var sut = CreateManageController();
            var result = await sut.Index();

            MockApiHelper.Verify(x => x.GetFromUri<UserDetailsModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.NotZero(sut.ModelState.ErrorCount, "Error count failed.");
        }

        [Test]
        public async Task GET_Index_NoErrors_ReturnsViewResult()
        {
            MockApiHelper.Setup(x => x.GetFromUri<UserDetailsModel>(It.IsAny<string>()))
                .ReturnsAsync(new UserDetailsModel {StatusMessage = "OK"});

            var sut = CreateManageController();
            var result = await sut.Index();

            MockApiHelper.Verify(x => x.GetFromUri<UserDetailsModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.Zero(sut.ModelState.ErrorCount, "Error count failed.");
        }

        [Test]
        public async Task POST_Index_InvalidModel_ReturnsViewResult()
        {
            var sut = CreateManageController();
            sut.ModelState.AddModelError("ErrorCode", "ErrorDescription");
            var result = await sut.Index(new UserDetailsModel());

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<UserDetailsModel>(((ViewResult) result).Model);
            Assert.NotZero(sut.ModelState.ErrorCount, "Error count failed.");
        }

        [Test]
        public async Task POST_Index_SetUserDetailsHasErrors_ReturnsViewResultWithErrorMessage()
        {
            MockApiHelper.Setup(x => x.PostAsync(It.IsAny<UserDetailsModel>(), It.IsAny<string>()))
                .ReturnsAsync(new UserDetailsModel {Errors = TestErrors, StatusMessage = "Error"});

            var sut = CreateManageController();
            var result = await sut.Index(new UserDetailsModel());

            MockApiHelper.Verify(x => x.PostAsync(It.IsAny<UserDetailsModel>(), It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<UserDetailsModel>(((ViewResult) result).Model);
            Assert.NotZero(sut.ModelState.ErrorCount, "Error count failed.");
        }

        [Test]
        public async Task POST_Index_SetUserDetailsHasNoErrors_ReturnsViewResultWithErrorMessage()
        {
            MockApiHelper.Setup(x => x.PostAsync(It.IsAny<UserDetailsModel>(), It.IsAny<string>()))
                .ReturnsAsync(new UserDetailsModel { StatusMessage = "OK"});

            var sut = CreateManageController();
            var result = await sut.Index(new UserDetailsModel());

            MockApiHelper.Verify(x => x.PostAsync(It.IsAny<UserDetailsModel>(), It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<UserDetailsModel>(((ViewResult) result).Model);
            Assert.Zero(sut.ModelState.ErrorCount, "Error count failed.");
            Assert.AreEqual(Resources.ControllerStrings.ManageController.UserDetailsUpdated, ((UserDetailsModel)((ViewResult)result).Model).StatusMessage);
        }
    }
}
