using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using Pegasus.Extensions;

namespace PegasusTests.ExtensionTests
{
    class HttpRequestExtensionTests
    {
        [Test]
        public void IsAjaxRequest_WithAjaxHeaders_ReturnsTrue()
        {
            HttpContext context = new DefaultHttpContext();
            var request =  context.Request;
            request.Headers["X-Requested-With"] = "XMLHttpRequest";

            var result = request.IsAjaxRequest();

            Assert.IsTrue(result);
        }

        [Test]
        public void IsAjaxRequest_WithoutAjaxtHeaders_ReturnsFalse()
        {
            HttpContext context = new DefaultHttpContext();
            var request = context.Request;

            var result = request.IsAjaxRequest();

            Assert.IsFalse(result);
        }
    }
}
