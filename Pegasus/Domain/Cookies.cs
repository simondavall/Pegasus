using System;
using Microsoft.AspNetCore.Http;
using Pegasus.Models.Settings;

namespace Pegasus.Domain
{
    public class Cookies
    {
        private readonly ISettingsModel _settingsModel;

        public Cookies(ISettingsModel settingsModel)
        {
            _settingsModel = settingsModel;
        }

        public void WriteCookie(HttpResponse response, string setting, string settingValue)
        {
            WriteCookie(response, setting, settingValue, _settingsModel.CookieExpiryDays);
        }

        public void WriteCookie(HttpResponse response, string setting, string settingValue, int expiryDays)
        {
            if (expiryDays == 0)
                expiryDays = _settingsModel.CookieExpiryDays;
            var options = new CookieOptions { Expires = new DateTimeOffset(DateTime.Now.AddDays(expiryDays)) };
            response.Cookies.Append(setting, settingValue, options);
        }
    }
}
