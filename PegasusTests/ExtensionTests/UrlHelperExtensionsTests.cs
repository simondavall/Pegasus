using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Pegasus.Controllers;
using Pegasus.Extensions;
using Pegasus.Library.Api;
using Pegasus.Services;
using PegasusTests.TestUtilities;

namespace PegasusTests.ExtensionTests
{
    class UrlHelperExtensionsTests
    {
        [Test]
        public void ResetPasswordBaseUrl()
        {
            var mockLogger = new Mock<ILogger<AccountController>>().Object;
            var mockApiHelper = new Mock<IApiHelper>().Object;
            var mockSignInManager = new Mock<ISignInManager>().Object;
            var mockAccountsEndpoint = new Mock<IAccountsEndpoint>().Object;
            var mockAuthenticationEndpoint = new Mock<IAuthenticationEndpoint>().Object;

            var controller = new AccountController(mockLogger, mockApiHelper, mockSignInManager, mockAccountsEndpoint, mockAuthenticationEndpoint);

            var mockUrlHelper = new Mock<IUrlHelper>(MockBehavior.Loose);
            mockUrlHelper
                .Setup(x => x.Action(It.IsAny<UrlActionContext>()))
                .Returns("https//localhost")
                .Verifiable();


            controller.Url = mockUrlHelper.Object;

            var sut = controller.Url.ResetPasswordBaseUrl("https");

            Assert.AreEqual("https//localhost",sut);
        }
    }
}
