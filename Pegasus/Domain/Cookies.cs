using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Pegasus.Domain
{
    public class Cookies
    {
        private const int FallbackExpiryDate = 30;
        private readonly Settings _settings;

        public Cookies(IConfiguration configuration)
        {
            _settings = new Settings(configuration);
        }

        public void WriteCookie(HttpResponse response, string setting, string settingValue)
        {
            var cookieExpiryDays = GetDefaultExpiryDays();
            WriteCookie(response, setting, settingValue, cookieExpiryDays);
        }

        public void WriteCookie(HttpResponse response, string setting, string settingValue, int expiryDays)
        {
            if (expiryDays == 0)
                expiryDays = GetDefaultExpiryDays();
            var options = new CookieOptions { Expires = new DateTimeOffset(DateTime.Now.AddDays(expiryDays)) };
            response.Cookies.Append(setting, settingValue, options);
        }

        private int GetDefaultExpiryDays()
        {
            return _settings.GetSetting("cookieExpiryDays", FallbackExpiryDate);
        }
    }
}
