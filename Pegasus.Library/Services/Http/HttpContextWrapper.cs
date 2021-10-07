using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Pegasus.Library.Services.Http
{
    public interface IHttpContextWrapper
    {
        Task<AuthenticateResult> AuthenticateAsync(string scheme);

        Task<string> GetTokenAsync(string accessToken);

        Task SignInAsync(ClaimsPrincipal principal, AuthenticationProperties props);
        Task SignInAsync(string subject, ClaimsPrincipal principal);
        Task SignInAsync(string subject, ClaimsPrincipal principal, AuthenticationProperties props);

        Task SignOutAsync();
        Task SignOutAsync(string scheme);

        HttpResponse Response { get; }
        HttpRequest Request { get; }
        ClaimsPrincipal User { get; set; }
        
    }

    public class HttpContextWrapper : IHttpContextWrapper
    {
        private readonly HttpContext _httpContext;

        public HttpContextWrapper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor.HttpContext;
        }

        public HttpContextWrapper(ControllerBase controller)
        {
            _httpContext = controller.HttpContext;
        }

        public async Task<AuthenticateResult> AuthenticateAsync(string scheme)
        {
            return await _httpContext.AuthenticateAsync(scheme);
        }

        public async Task<string> GetTokenAsync(string accessToken)
        {
            return await _httpContext.GetTokenAsync(accessToken);
        }

        public async Task SignInAsync(ClaimsPrincipal principal, AuthenticationProperties props)
        {
            await _httpContext.SignInAsync(principal, props);
        }

        public async Task SignInAsync(string subject, ClaimsPrincipal principal, AuthenticationProperties props)
        {
            await _httpContext.SignInAsync(subject, principal, props);
        }

        public async Task SignInAsync(string subject, ClaimsPrincipal principal)
        {
            await _httpContext.SignInAsync(subject, principal);
        }

        public async Task SignOutAsync()
        {
            await _httpContext.SignOutAsync();
        }

        public async Task SignOutAsync(string scheme)
        {
            await _httpContext.SignOutAsync(scheme);
        }

        public HttpResponse Response
        {
            get { return _httpContext.Response; }
        }

        public HttpRequest Request
        {
            get { return _httpContext.Request; }
        }

        public ClaimsPrincipal User
        {
            get { return _httpContext.User; }
            set { _httpContext.User = value; }
        }


    }
}
