using System;
using Microsoft.AspNetCore.Http;
using Pegasus.Services;

namespace Pegasus.Domain
{
    public class Cookies
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly int _cookieExpiryDays;

        public Cookies(IHttpContextAccessor httpContextAccessor, ISettingsService settings)
        {
            _httpContextAccessor = httpContextAccessor;
            _cookieExpiryDays = settings.CookieExpiryDays;
        }

        public void WriteCookie(HttpResponse response, string cookieName, string cookieData)
        {
            WriteCookie(response, cookieName, cookieData, _cookieExpiryDays);
        }

        public void WriteCookie(HttpResponse response, string cookieName, string cookieData, int expiryDays)
        {
            if (expiryDays == 0)
                expiryDays = _cookieExpiryDays;
            var options = new CookieOptions { Expires = new DateTimeOffset(DateTime.Now.AddDays(expiryDays)) };
            response.Cookies.Append(cookieName, cookieData, options);
        }

        public void DeleteCookie(HttpResponse response, string cookieName)
        {
            if (_httpContextAccessor.HttpContext.Request.Cookies.ContainsKey(cookieName))
                response.Cookies.Delete(cookieName);
        }
    }
}
