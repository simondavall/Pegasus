using System;
using Microsoft.AspNetCore.Http;
using Pegasus.Library.Services.Http;
using Pegasus.Services;

namespace Pegasus.Domain
{
    public interface ICookies
    {
        void WriteCookie(string cookieName, string cookieData);
        void WriteCookie(string cookieName, string cookieData, int expiryDays);
        void DeleteCookie(string cookieName);
    }

    public class Cookies : ICookies
    {
        private readonly IHttpContextWrapper _httpContext;
        private readonly int _cookieExpiryDays;

        public Cookies(IHttpContextWrapper httpContextWrapper, ISettingsService settings)
        {
            _httpContext = httpContextWrapper;
            _cookieExpiryDays = settings.Settings.CookieExpiryDays;
        }

        public void WriteCookie(string cookieName, string cookieData)
        {
            WriteCookie(cookieName, cookieData, _cookieExpiryDays);
        }

        public void WriteCookie(string cookieName, string cookieData, int expiryDays)
        {
            if (expiryDays == 0)
                expiryDays = _cookieExpiryDays;
            var options = new CookieOptions { Expires = new DateTimeOffset(DateTime.Now.AddDays(expiryDays)) };
            _httpContext.Response.Cookies.Append(cookieName, cookieData, options);
        }

        public void DeleteCookie(string cookieName)
        {
            if (_httpContext.Request.Cookies.ContainsKey(cookieName))
                _httpContext.Response.Cookies.Delete(cookieName);
        }
    }
}
