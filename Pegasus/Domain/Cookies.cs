using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Pegasus.Domain
{
    public class Cookies
    {
        private readonly Settings _settings;

        public Cookies(IConfiguration configuration)
        {
            _settings = new Settings(configuration);
        }

        public void WriteCookie(HttpResponse response, string setting, string settingValue)
        {
            const int thirtyDays = 30;
            var cookieExpiryDays = _settings.GetSetting("cookieExpiryDays", thirtyDays);
            var options = new CookieOptions { Expires = new DateTimeOffset(DateTime.Now.AddDays(cookieExpiryDays)) };
            response.Cookies.Append(setting, settingValue, options);
        }
    }
}
