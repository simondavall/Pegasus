using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Pegasus.Library.Api;

namespace PegasusTests
{
    public class AuthenticationTests
    {
        [Test]
        public void CallingAuthenticate()
        {
            var myJsonConfig = "{\"apiRoot\": \"https://pegasus2api.local\" }";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(myJsonConfig));

            var configuration = new ConfigurationBuilder()
               .AddJsonStream(stream)
               .Build();

            var apiHelper = new ApiHelper(configuration);
            var username = "simon.davall@gmail.com";
            var password = "H3r3ford!!";
            var sut = apiHelper.Authenticate(username, password).Result;

            Assert.IsNotNull(sut);
            Assert.AreEqual(username, sut.Username);
        }
    }
}
