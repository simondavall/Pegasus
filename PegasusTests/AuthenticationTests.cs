using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Pegasus.Library.Api;
using Pegasus.Library.JwtAuthentication.Models;
using Pegasus.Library.Models.Account;

namespace PegasusTests
{
    public class AuthenticationTests
    {
        readonly AuthenticatedUser _authenticatedUser = new AuthenticatedUser
        {
            Username = "valid_username",
            UserId = "valid_user_id",
            AccessToken = "valid_access_token"
        };

        [Test]
        public void Authenticate_WithGoodCredentials_ReturnsAccessToken()
        {
            var apiHelper = CreateApiHelper(GetValidResponse());
            var authenticationEndpoint = new AuthenticationEndpoint(apiHelper);
            
            var sut = authenticationEndpoint.Authenticate((UserCredentials)null).Result;

            Assert.IsNotNull(sut);
            Assert.AreEqual(_authenticatedUser.Username, sut.Username);
            Assert.IsTrue(sut.Authenticated, "Returned user is not authenticated.");
            Assert.IsNotNull(sut.AccessToken, "Access token was not returned.");
        }
        
        [Test]
        public void Authenticate_WithBadCredentials_ReturnsNullAccessToken()
        {
            var response = new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest };
            var apiHelper = CreateApiHelper(response);
            var authenticationEndpoint = new AuthenticationEndpoint(apiHelper);

            var sut = authenticationEndpoint.Authenticate((UserCredentials)null).Result;

            Assert.IsNotNull(sut);
            Assert.IsFalse(sut.Authenticated, "Returned user is authenticated.");
            Assert.IsNull(sut.AccessToken, "Access token was returned.");
            Assert.AreEqual("Bad Request", sut.FailedReason );
        }
        
        [Test]
        public void Authenticate_WithGoodUserId_ReturnsAccessToken()
        {
            var apiHelper = CreateApiHelper(GetValidResponse());
            var authenticationEndpoint = new AuthenticationEndpoint(apiHelper);
            
            var sut = authenticationEndpoint.Authenticate(_authenticatedUser.UserId).Result;

            Assert.IsNotNull(sut);
            Assert.AreEqual(_authenticatedUser.Username, sut.Username);
            Assert.IsTrue(sut.Authenticated, "Returned user is not authenticated.");
            Assert.IsNotNull(sut.AccessToken, "Access token was not returned.");
        }
        
        [Test]
        public void AuthenticateWith2fa_WithGoodUserId_ReturnsAccessToken()
        {
            var apiHelper = CreateApiHelper(GetValidResponse());
            var authenticationEndpoint = new AuthenticationEndpoint(apiHelper);
            
            var sut = authenticationEndpoint.Authenticate2Fa(_authenticatedUser.UserId).Result;

            Assert.IsNotNull(sut);
            Assert.AreEqual(_authenticatedUser.Username, sut.Username);
            Assert.IsTrue(sut.Authenticated, "Returned user is not authenticated.");
            Assert.IsNotNull(sut.AccessToken, "Access token was not returned.");
        }

        private static ApiHelper CreateApiHelper(HttpResponseMessage response)
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);
            var apiHelper = new ApiHelper(null, null, mockHttpMessageHandler.Object);
            return apiHelper;
        }

        private HttpResponseMessage GetValidResponse()
        {
            var response = new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.OK;
            response.Content = new ObjectContent<AuthenticatedUser>(_authenticatedUser, new JsonMediaTypeFormatter());

            return response;
        }
    }
}
