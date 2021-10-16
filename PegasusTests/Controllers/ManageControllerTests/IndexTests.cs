using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Pegasus.Library.Models.Manage;
using ManageControllerStrings = Pegasus.Library.Services.Resources.Resources.ControllerStrings.ManageController;

namespace PegasusTests.Controllers.ManageControllerTests
{
    internal class IndexTests : ManageControllerTestsBase
    {
        [Test]
        public async Task GET_Index_HasErrors_ReturnsViewResultWithErrorMessage()
        {
            MockManageEndpoint.Setup(x => x.GetUserDetails(It.IsAny<string>()))
                .ReturnsAsync(new UserDetailsModel { Errors = TestErrors, StatusMessage = "Error" });

            var sut = CreateManageController();
            var result = await sut.Index();

            result.Should().BeOfType<ViewResult>();
            sut.ModelState.ErrorCount.Should().Be(1);
        }

        [Test]
        public async Task GET_Index_NoErrors_ReturnsViewResult()
        {
            MockManageEndpoint.Setup(x => x.GetUserDetails(It.IsAny<string>()))
                .ReturnsAsync(new UserDetailsModel { StatusMessage = "OK" });

            var sut = CreateManageController();
            var result = await sut.Index();

            result.Should().BeOfType<ViewResult>();
            sut.ModelState.ErrorCount.Should().Be(0);
        }

        [Test]
        public async Task POST_Index_InvalidModel_ReturnsViewResult()
        {
            var sut = CreateManageController();
            sut.ModelState.AddModelError("ErrorCode", "ErrorDescription");
            var result = await sut.Index(new UserDetailsModel());

            result.Should().BeOfType<ViewResult>();
            ((ViewResult)result).Model.Should().BeOfType<UserDetailsModel>();
            sut.ModelState.ErrorCount.Should().Be(1);
        }

        [Test]
        public async Task POST_Index_SetUserDetailsHasErrors_ReturnsViewResultWithErrorMessage()
        {
            MockManageEndpoint.Setup(x => x.SetUserDetails(It.IsAny<UserDetailsModel>()))
                .ReturnsAsync(new UserDetailsModel { Errors = TestErrors, StatusMessage = "Error" });

            var sut = CreateManageController();
            var result = await sut.Index(new UserDetailsModel());

            result.Should().BeOfType<ViewResult>();
            ((ViewResult)result).Model.Should().BeOfType<UserDetailsModel>();
            sut.ModelState.ErrorCount.Should().Be(2);
        }

        [Test]
        public async Task POST_Index_SetUserDetailsHasNoErrors_ReturnsViewResultWithErrorMessage()
        {
            MockManageEndpoint.Setup(x => x.SetUserDetails(It.IsAny<UserDetailsModel>()))
                .ReturnsAsync(new UserDetailsModel { StatusMessage = "OK" });

            var sut = CreateManageController();
            var result = await sut.Index(new UserDetailsModel());

            result.Should().BeOfType<ViewResult>();
            ((ViewResult)result).Model.Should().BeOfType<UserDetailsModel>();
            sut.ModelState.ErrorCount.Should().Be(0);
            var userDetailsModel = (UserDetailsModel)((ViewResult)result).Model;
            userDetailsModel.StatusMessage.Should().Be(ManageControllerStrings.UserDetailsUpdated);
        }
    }
}