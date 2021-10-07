using System;
using System.Collections.Specialized;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using Moq;
using Pegasus.Services;

namespace PegasusTests
{
    class SettingsServiceTests
    {
        private IConfiguration _configuration;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;

        [SetUp]
        public void SetupOnEachTest()
        {
            // setup configuration from json string
            const string myJsonConfig = "{\"PegasusSettings\":{\"TaskFilterId\":125,\"CookieExpiryDays\":20,\"PageSize\":25}}";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(myJsonConfig));

            _configuration = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();

            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockHttpContextAccessor.Setup(x => x.HttpContext.Request.Cookies[It.IsAny<string>()])
                .Returns(
                    "{\"CookieExpiryDays\":180,\"PaginationEnabled\":true,\"ProjectId\":2,\"TaskFilterId\":17}");
        }

        [Test]
        public void SettingsService_SettingResidesInCookie_ReturnsCookieValue()
        {
            var sut = new SettingsService(_mockHttpContextAccessor.Object, _configuration);
            var result = sut.Settings.TaskFilterId;
            Assert.AreEqual(17, result);
        }

        [Test]
        public void SettingsService_SettingResidesInConfigButNotCookie_ReturnsConfigValue()
        {
            var sut = new SettingsService(_mockHttpContextAccessor.Object, _configuration);
            var result = sut.Settings.PageSize;
            Assert.AreEqual(25, result);
        }

        [Test]
        public void SettingsService_NullSettingName_ThrowsArgumentNullException()
        {
            var sut = new SettingsService(_mockHttpContextAccessor.Object, _configuration);
            Assert.Throws<ArgumentNullException>(() => sut.GetSetting<int>(null));
        }

        [Test]
        public void SettingsService_SettingDoesNotExist_ThrowsPropertyNotFoundException()
        {
            var sut = new SettingsService(_mockHttpContextAccessor.Object, _configuration);
            Assert.Throws<PropertyNotFoundException>(() => sut.GetSetting<int>("DoesNotExist"));
        }

        [Test]
        public void SettingsService_SettingWrongType_ThrowsInvalidTypeException()
        {
            var sut = new SettingsService(_mockHttpContextAccessor.Object, _configuration);
            Assert.Throws<PropertyTypeInvalidException>(() => sut.GetSetting<string>("TaskFilterId"));
        }

        [Test]
        public void GetProperty_ValidProperty_ReturnsPropertyValue()
        {
            _mockHttpContextAccessor.Setup(x => x.HttpContext.Request.Query[It.IsAny<string>()]).Returns("");
            var sut = new SettingsService(_mockHttpContextAccessor.Object, _configuration);
            Assert.AreEqual(17, sut.GetSetting<int>("TaskFilterId"));
        }

        [Test]
        public void GetSetting_QueryStringPresent_ReturnsQueryStringValue()
        {
            _mockHttpContextAccessor.Setup(x => x.HttpContext.Request.Query["TaskFilterId"]).Returns("145");
            _mockHttpContextAccessor
                .Setup(x => x.HttpContext.Response.Cookies.Append(It.IsAny<string>(), It.IsAny<string>())).Verifiable();

            var sut = new SettingsService(_mockHttpContextAccessor.Object, _configuration);
            Assert.AreEqual(145, sut.GetSetting<int>("TaskFilterId"));
        }

        [Test]
        public void LoadSettingsFromCookies_DoesNotFindSettingsInCookie_ReturnsEmptyDictionary()
        {
            // by implication. Setting cookies to empty string will force LoadSettingsFromCookies to return default value
            _mockHttpContextAccessor.Setup(x => x.HttpContext.Request.Query[It.IsAny<string>()]).Returns("");
            _mockHttpContextAccessor.Setup(x => x.HttpContext.Request.Cookies[It.IsAny<string>()])
                .Returns("");
            var sut = new SettingsService(_mockHttpContextAccessor.Object, _configuration);

            Assert.AreEqual(125, sut.GetSetting<int>("TaskFilterId"));
        }

        [Test]
        public void SettingsService_SettingNotInCookieOrConfig_ReturnsDefaultValue()
        {
            const string myJsonConfig = "{\"PegasusSettings\":{\"TaskFilterId\":125,\"CookieExpiryDays\":20}}";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(myJsonConfig));
            _configuration = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();

            var sut = new SettingsService(_mockHttpContextAccessor.Object, _configuration);
            var result = sut.Settings.PageSize;
            Assert.AreEqual(0, result);
        }
    }
}
