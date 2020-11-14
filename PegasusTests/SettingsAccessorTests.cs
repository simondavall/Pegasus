using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Pegasus.Domain;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace PegasusTests
{
    class SettingsAccessorTests
    {
        private IConfiguration _configuration;

        [SetUp]
        public void Setup()
        {
            // setup configuration from json string
            var myJsonConfig = "{\"PegasusSettings\": {\"taskFilterId\": \"125\",\"cookieExpiryDays\":  20}}";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(myJsonConfig));

            _configuration = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();
        }

        [Test]
        public void Settings_TestInMemoryConfig_ReturnsDefaultValue()
        {
            var sut = new SettingsAccessor(_configuration);
            var result = sut.GetSetting("taskFilterId", 30);
            Assert.AreEqual(125, result);
        }

        [Test]
        public void Settings_NonExistentSetting_ReturnsDefaultValue()
        {
            var sut = new SettingsAccessor(_configuration);
            var result = sut.GetSetting("DoesNotExist", 30);
            Assert.AreEqual(30, result);
        }

        [Test]
        public void Settings_RequestWithSetting_ReturnsRequestSetting()
        {
            var httpContext = new DefaultHttpContext();
            var request = httpContext.Request;
            request.QueryString = new QueryString("?taskFilterId=789");

            var sut = new SettingsAccessor(_configuration);
            var result = sut.GetSetting(request, "taskFilterId", 35);
            Assert.AreEqual(789, result);
        }

        [Test]
        public void Settings_RequestWithNoSetting_ReturnsConfigSetting()
        {
            var httpContext = new DefaultHttpContext();
            var request = httpContext.Request;

            var sut = new SettingsAccessor(_configuration);
            var result = sut.GetSetting(request, "taskFilterId", 35);
            Assert.AreEqual(125, result);
        }

        [Test]
        public void Settings_RequestWithNoSettingAndNoConfig_ReturnsDefaultValue()
        {
            var httpContext = new DefaultHttpContext();
            var request = httpContext.Request;

            var sut = new SettingsAccessor(_configuration);
            var result = sut.GetSetting(request, "DoesNotExist", 35);
            Assert.AreEqual(35, result);
        }

        [Test]
        public void SettingsFromConfig_RequestWithWrongType_ReturnsDefaultValue()
        {
            // setup configuration from json string
            var myJsonConfig = "{\"PegasusSettings\": {\"taskFilterId\": \"TextSetting\",\"cookieExpiryDays\":  20}}";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(myJsonConfig));

            _configuration = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();

            var sut = new SettingsAccessor(_configuration);
            var result = sut.GetSetting("taskFilterId", 35);
            Assert.AreEqual(35, result);
        }

        [Test]
        public void SettingsFromCookie_RequestWithValidKey_ReturnsCorrectValue()
        {
            var httpContext = new DefaultHttpContext();
            var request = httpContext.Request;
            request.Headers["Cookie"] = new[] {"testKey=1234"};

            var sut = new SettingsAccessor(_configuration);
            var result = sut.GetSetting(request, "testKey", 4321);

            Assert.AreEqual(1234, result);
        }

        [Test]
        public void SettingsFromCookie_RequestWithInvalidKey_ReturnsDefaultValue()
        {
            var httpContext = new DefaultHttpContext();
            var request = httpContext.Request;
            request.Headers["Cookie"] = new[] {"testKey=1234"};
            
            var sut = new SettingsAccessor(_configuration);
            var result = sut.GetSetting(request, "WrongKey", 4321);
            Assert.AreEqual(4321, result);
        }

        [Test]
        public void SettingsFromCookie_RequestForPoorlyTypedValue_ReturnsDefaultValue()
        {
            var httpContext = new DefaultHttpContext();
            var request = httpContext.Request;
            request.Headers["Cookie"] = new[] {"testKey=stringValue"};
            
            var sut = new SettingsAccessor(_configuration);
            var result = sut.GetSetting(request, "testKey", 4321);
            Assert.AreEqual(4321, result);
        }

        [Test]
        public void SettingsFromCookie_RequestForWithEmptyCookieValue_ReturnsDefaultValue()
        {
            var httpContext = new DefaultHttpContext();
            var request = httpContext.Request;
            request.Headers["Cookie"] = new[] {"testKey="};
            
            var sut = new SettingsAccessor(_configuration);
            var result = sut.GetSetting(request, "testKey", 4321);
            Assert.AreEqual(4321, result);
        }

        [Test]
        public void SettingsFromCookie_RequestForWithEmptyCookieValueAndNoDefault_ReturnsNull()
        {
            var httpContext = new DefaultHttpContext();
            var request = httpContext.Request;
            request.Headers["Cookie"] = new[] {"testKey="};
            
            var sut = new SettingsAccessor(_configuration);
            var result = sut.GetSetting<string>(request, "testKey");
            Assert.AreEqual(null, result);
        }
    }
}
