using NUnit.Framework;

namespace PegasusTests.ApiTests
{
    public class ManageApiTests
    {
        [Test]
        public void DefaultTest()
        {
            Assert.Pass("Need to implement manage api tests");
        }

        [Test]
        public void ApiHelper_CallNonExistentUri_ReturnsErrorCode()
        {
            //var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            //mockHttpContextAccessor.Setup(x => x.HttpContext.GetTokenAsync(It.IsAny<string>()))
            //    .Returns(Task.FromResult("valid_access_token"));
            ////.HttpContext.GetTokenAsync("access_token")
            //var mockConfiguration = new Mock<IConfiguration>();
            //mockConfiguration.Setup(x => x["apiRoot"]).Returns("https://baseUrl/");

            //var apiHelper = new ApiHelper(mockConfiguration.Object, mockHttpContextAccessor.Object);

            //var something = apiHelper.GetListFromUri<string>("api/DoesNotExist");

            Assert.Pass("This need to be implemented with the GetTokenAsync mock");
        }
    }
}
