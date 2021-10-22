using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using PegasusService;
using PegasusTests.TestUtilities;

namespace PegasusTests
{
    
    class KeepAliveServiceTests
    {
        private HttpClient _client;
        private IConfiguration _configuration;
        private readonly ILogger _logger;

        public KeepAliveServiceTests(ILogger logger)
        {
            _logger = logger;
        }

        public KeepAliveServiceTests()
        {

        }

        [OneTimeSetUp]
        public void KeepAlive_FixtureSetup()
        {
            _client = FakeHttpClient(new[] {"https://www.google.com/", "http://example.org/test"}, HttpStatusCode.OK);

            // setup configuration from json string
            var myJsonConfig = "{\"KeepAlive\": {\"Sites\": \"https://pegasus.local;https://google.com\",\"TaskDelay\": \"5000\"}}";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(myJsonConfig));

            _configuration = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();
        }

        [OneTimeTearDown]
        public void KeepAlive_Teardown()
        {
            _client.Dispose();
        }

        [Test]
        public async Task KeepAlive_SiteOK_ReturnsTrue()
        {
            var sut = new KeepAliveService(_client, _configuration, _logger);
            var result = await sut.KeepAlive("http://example.org/test", new CancellationToken());
            Assert.IsTrue(result);
        }

        [Test]
        public async Task KeepAlive_SiteNotFound_ReturnsFalse()
        {
            var sut = new KeepAliveService(_client, _configuration, _logger);
            var result = await sut.KeepAlive("http://example.org/notthere", new CancellationToken());
            Assert.IsFalse(result);
        }

        [Test]
        public async Task KeepAlive_TestFakeHttpClientCall()
        {
            var response1 = await _client.GetAsync("http://example.org/notthere");
            var response2 = await _client.GetAsync("http://example.org/test");

            Assert.AreEqual(HttpStatusCode.NotFound, response1.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, response2.StatusCode);
        }

        private HttpClient FakeHttpClient(IEnumerable<string> urls, HttpStatusCode statusCode)
        {
            var fakeResponseHandler = new FakeResponseHandler();
            foreach (var uri in urls)
            {
                fakeResponseHandler.AddFakeResponse(new Uri(uri), new HttpResponseMessage(statusCode));
            }
            
            return new HttpClient(fakeResponseHandler);
        }
    }
}
