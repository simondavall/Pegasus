using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using Pegasus.Domain;
using Pegasus.Library.Services.Http;
using Pegasus.Services;
using Pegasus.Services.Models;

namespace PegasusTests.Domain
{
    class CookiesTests
    {
        private Mock<IHttpContextWrapper> _mockHttpContextWrapper;
        private Mock<ISettingsService> _mockSettingsService;

        [SetUp]
        public void SetupOnEachTest()
        {
            _mockHttpContextWrapper = new Mock<IHttpContextWrapper>();
            _mockSettingsService = new Mock<ISettingsService>();
            _mockHttpContextWrapper.Setup(x => x.Request.Cookies[It.IsAny<string>()])
                .Returns(
                    "{\"CookieExpiryDays\":180,\"PaginationEnabled\":true,\"ProjectId\":2,\"TaskFilterId\":17}");

            _mockSettingsService.SetupGet(x => x.Settings).Returns(new SettingsModel() { CookieExpiryDays = 30 });
            _mockHttpContextWrapper.SetupAllProperties();
        }

        [Test]
        public void Cookies_WriteCookie_ExecutesWriteCookieOnContext()
        {
            _mockHttpContextWrapper.Setup(x => x.Response.Cookies.Append(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CookieOptions>())).Verifiable();

            var sut = new Cookies(_mockHttpContextWrapper.Object, _mockSettingsService.Object);
            sut.WriteCookie("cookie-name", "cookie-data");

            _mockHttpContextWrapper.Verify(x => x.Response.Cookies.Append(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CookieOptions>()), Times.Exactly(1));
        }

        [Test]
        public void Cookies_WriteCookieUsingExpiry_ExecutesWriteCookieOnContext()
        {
            _mockHttpContextWrapper.Setup(x => x.Response.Cookies.Append(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CookieOptions>())).Verifiable();

            var sut = new Cookies(_mockHttpContextWrapper.Object, _mockSettingsService.Object);
            sut.WriteCookie("cookie-name", "cookie-data", 0);

            _mockHttpContextWrapper.Verify(x => x.Response.Cookies.Append(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CookieOptions>()), Times.Exactly(1));

        }

        
        [Test]
        public void Cookies_DeleteCookie_ExecutesDeleteCookieOnContext()
        {
            _mockHttpContextWrapper.Setup(x => x.Request.Cookies.ContainsKey(It.IsAny<string>())).Returns(true).Verifiable();
            _mockHttpContextWrapper.Setup(x => x.Response.Cookies.Delete(It.IsAny<string>())).Verifiable();

            var sut = new Cookies(_mockHttpContextWrapper.Object, _mockSettingsService.Object);
            sut.DeleteCookie("cookie-name");

            _mockHttpContextWrapper.Verify(x => x.Request.Cookies.ContainsKey(It.IsAny<string>()), Times.Exactly(1));
            _mockHttpContextWrapper.Verify(x => x.Response.Cookies.Delete(It.IsAny<string>()), Times.Exactly(1));
        }
    }
}
