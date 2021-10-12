using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Pegasus.Controllers;

namespace PegasusTests.Controllers
{
    class AboutControllerTests
    {
        [Test]
        public void Index_ReturnsViewResult()
        {
            var sut = new AboutController();
            var result = sut.Index();

            Assert.IsInstanceOf<ViewResult>(result);
        }
    }
}
