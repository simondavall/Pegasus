using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Pegasus.Domain
{
    public class Settings
    {
        private readonly IConfiguration _configuration;

        public Settings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public int GetSetting(HttpRequest request, string settingName, int defaultReturnVal = 0)
        {
            if (request != null)
            {
                if (TryGetFromQueryString(request, settingName, out var id))
                    return id;

                if (TryGetFromCookie(request, settingName, out id))
                    return id;
            }

            return GetSetting(settingName, defaultReturnVal);
        }

        public int GetSetting(string settingName, int defaultReturnVal = 0)
        {
            return TryGetFromConfiguration(settingName, out var id) ? id : defaultReturnVal;
        }

        private bool TryGetFromQueryString(HttpRequest request, string settingName, out int id)
        {
            return IsValidSetting(request.Query[settingName], out id);
        }

        private bool TryGetFromCookie(HttpRequest request, string settingName, out int id)
        {
            return IsValidSetting(request.Cookies[settingName], out id);
        }

        private bool TryGetFromConfiguration(string settingName, out int id)
        {
            return IsValidSetting(_configuration[$"DefaultSettings:{settingName}"], out id);
        }

        private bool IsValidSetting(string setting, out int id)
        {
            return int.TryParse(setting, out id);
        }


    }
}
