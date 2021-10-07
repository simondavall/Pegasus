using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using Pegasus.Domain;
using Pegasus.Library.JwtAuthentication.Constants;
using Pegasus.Services;
using Pegasus.Services.Models;

namespace PegasusTests.ServiceTests
{
    class AnalyticsServiceTests
    {
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private Mock<ISettingsService> _mockSettingsService;
        private Mock<ICookies> _mockCookies;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockSettingsService = new Mock<ISettingsService>();
            _mockCookies = new Mock<ICookies>();

            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());
        }

        [Test]
        public void SaveAnalyticsData_AnalyticsCookieAllowed_SavesDataToCookie()
        {
            var settingsModel = new SettingsModel { AnalyticsCookieEnabled = true };
            _mockSettingsService.Setup(x => x.Settings).Returns(settingsModel);
            _mockCookies.Setup(x => x.WriteCookie(It.IsAny<HttpResponse>(), CookieConstants.Analytics, It.IsAny<string>()));

            var sut = new TestableAnalyticsService(_mockHttpContextAccessor.Object, _mockSettingsService.Object, _mockCookies.Object);
            sut.SaveAnalyticsData("Some data");

            _mockCookies.Verify(
                x => x.WriteCookie(It.IsAny<HttpResponse>(), CookieConstants.Analytics, It.IsAny<string>()),
                Times.Exactly(1));
        }

        [Test]
        public void SaveAnalyticsData_AnalyticsCookieNotAllowed_DeleteCookie()
        {
            var settingsModel = new SettingsModel { AnalyticsCookieEnabled = false };
            _mockSettingsService.Setup(x => x.Settings).Returns(settingsModel);

            _mockCookies.Setup(x => x.DeleteCookie(It.IsAny<HttpResponse>(), CookieConstants.Analytics));

            var sut = new TestableAnalyticsService(_mockHttpContextAccessor.Object, _mockSettingsService.Object, _mockCookies.Object);
            sut.SaveAnalyticsData("Some data");

            _mockCookies.Verify(
                x => x.DeleteCookie(It.IsAny<HttpResponse>(), CookieConstants.Analytics),
                Times.Exactly(1));
        }

        private class TestableAnalyticsService : AnalyticsService
        {
            public TestableAnalyticsService(IHttpContextAccessor httpContextAccessor, ISettingsService settingsService, ICookies cookies = null) : base(httpContextAccessor, settingsService)
            {
                if(!(cookies is null))
                    Cookies = cookies;
            }
        }
    }
}
