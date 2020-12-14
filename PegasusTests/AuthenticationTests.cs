//using System.Security.Claims;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Http;
//using Moq;
using NUnit.Framework;

namespace PegasusTests
{
    public class AuthenticationTests
    {
        [Test]
        public void CallingAuthenticate()
        {
            Assert.Pass("Need to implement Authentication Testing");

            // Have a look at implementing this https://github.com/richardszalay/mockhttp in order to mock HttpClient.

            //var myJsonConfig = "{\"apiRoot\": \"https://pegasus2api.local\" }";
            //var stream = new MemoryStream(Encoding.UTF8.GetBytes(myJsonConfig));

            //var configuration = new ConfigurationBuilder()
            //   .AddJsonStream(stream)
            //   .Build();

            ////TODO Urgent!! Change this to use a mock authentication rather than real credentials
            //var apiHelper = new ApiHelper(configuration);
            //var credentials = new UserCredentials {Username = "simon.davall@gmail.com", Password = "H3r3ford!!"};
            //var sut = apiHelper.Authenticate(credentials).Result;

            //Assert.IsNotNull(sut);
            //Assert.AreEqual(credentials.Username, sut.Username);
        }


        ///// <summary>
        ///// Need this to mock the GetTokenAsync function which returns the access token.
        ///// </summary>
        ///// <param name="httpContextAccessorMock"></param>
        ///// <param name="tokenName"></param>
        ///// <param name="tokenValue"></param>
        ///// <param name="scheme"></param>
        //private void MockHttpContextGetToken(
        //    Mock<IHttpContextAccessor> httpContextAccessorMock,
        //    string tokenName, string tokenValue, string scheme = null)
        //{
        //    var authenticationServiceMock = new Mock<IAuthenticationService>();
        //    httpContextAccessorMock
        //        .Setup(x => x.HttpContext.RequestServices.GetService(typeof(IAuthenticationService)))
        //        .Returns(authenticationServiceMock.Object);

        //    var authResult = AuthenticateResult.Success(
        //        new AuthenticationTicket(new ClaimsPrincipal(), scheme));

        //    authResult.Properties.StoreTokens(new[]
        //    {
        //        new AuthenticationToken { Name = tokenName, Value = tokenValue }
        //    });

        //    authenticationServiceMock
        //        .Setup(x => x.AuthenticateAsync(httpContextAccessorMock.Object.HttpContext, scheme))
        //        .ReturnsAsync(authResult);
        //}
    }
}
