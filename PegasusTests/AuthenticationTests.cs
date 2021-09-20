using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Pegasus.Library.Api;
using Pegasus.Library.Models.Account;

namespace PegasusTests
{
    public class AuthenticationTests
    {
        [Test]
        public void Authenticate_WithGoodCredentials_ReturnsAccessToken()
        {
            // Have a look at implementing this https://github.com/richardszalay/mockhttp in order to mock HttpClient.
            
            var apiHelper = CreateApiHelper(null);
            var authenticationEndpoint = new AuthenticationEndpoint(apiHelper);

            var username = "simon.davall@gmail.com";
            var password = "H3r3ford!!";
            var credentials = new UserCredentials {Username = username, Password = password};
            
            var sut = authenticationEndpoint.Authenticate(credentials).Result;

            Assert.IsNotNull(sut);
            Assert.AreEqual(username, sut.Username);
            Assert.IsTrue(sut.Authenticated, "Returned user is not authenticated.");
            Assert.IsNotNull(sut.AccessToken, "Access token was not returned.");
        }
        
        [Test]
        public void Authenticate_WithBadCredentials_ReturnsNullAccessToken()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest});
            
            var apiHelper = CreateApiHelper(mockHttpMessageHandler.Object);
            var authenticationEndpoint = new AuthenticationEndpoint(apiHelper);
            var credentials = new UserCredentials {Username = "BadUsername", Password = "BadPassword"};

            var sut = authenticationEndpoint.Authenticate(credentials).Result;

            Assert.IsNotNull(sut);
            Assert.IsFalse(sut.Authenticated, "Returned user is authenticated.");
            Assert.IsNull(sut.AccessToken, "Access token was returned.");
        }
        
        private ApiHelper CreateApiHelper(HttpMessageHandler httpMessageHandler)
        {
            const string tokenName = "token_name";
            const string tokenValue = "valid_access_token";
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            MockHttpContextGetToken(mockHttpContextAccessor, tokenName, tokenValue);

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(x => x["apiRoot"]).Returns("https://pegasus2api.local/");

            var apiHelper = new ApiHelper(mockConfiguration.Object, mockHttpContextAccessor.Object, httpMessageHandler);
            return apiHelper;
        }
        
        //TODO Should be able to remove this when HttpResponses are faked.
        /// <summary>
        /// Need this to mock the GetTokenAsync function which returns the access token.
        /// </summary>
        private void MockHttpContextGetToken(Mock<IHttpContextAccessor> httpContextAccessorMock, string tokenName, string tokenValue, string scheme = null)
        {
            var authenticationServiceMock = new Mock<IAuthenticationService>();
            httpContextAccessorMock
                .Setup(x => x.HttpContext.RequestServices.GetService(typeof(IAuthenticationService)))
                .Returns(authenticationServiceMock.Object);
        
            var authResult = AuthenticateResult.Success(
                new AuthenticationTicket(new ClaimsPrincipal(), scheme));
        
            authResult.Properties.StoreTokens(new[]
            {
                new AuthenticationToken { Name = tokenName, Value = tokenValue }
            });
        
            authenticationServiceMock
                .Setup(x => x.AuthenticateAsync(httpContextAccessorMock.Object.HttpContext, scheme))
                .ReturnsAsync(authResult);
        }
    }
}
