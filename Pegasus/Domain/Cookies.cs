using System;
using Microsoft.AspNetCore.Http;
using Pegasus.Services;

namespace Pegasus.Domain
{
    public class Cookies
    {
        private readonly int _cookieExpiryDays;

        public Cookies(ISettingsService settings)
        {
            _cookieExpiryDays = settings.CookieExpiryDays;
        }

        public void WriteCookie(HttpResponse response, string setting, string settingValue)
        {
            WriteCookie(response, setting, settingValue, _cookieExpiryDays);
        }

        public void WriteCookie(HttpResponse response, string setting, string settingValue, int expiryDays)
        {
            if (expiryDays == 0)
                expiryDays = _cookieExpiryDays;
            var options = new CookieOptions { Expires = new DateTimeOffset(DateTime.Now.AddDays(expiryDays)) };
            response.Cookies.Append(setting, settingValue, options);
        }
    }
}
