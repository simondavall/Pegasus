using Pegasus.Domain;
using Pegasus.Library.JwtAuthentication.Constants;
using Pegasus.Library.Services.Http;

namespace Pegasus.Services
{
    public interface IMarketingService
    {
        void SaveMarketingData(string data);
    }

    public class MarketingService : IMarketingService
    {
        private readonly ISettingsService _settingsService;

        // create a marketing cookie to simulate a marketing scenario (might actually be a third party tool/plugin)
        protected ICookies Cookies;

        public MarketingService(IHttpContextWrapper httpContextWrapper, ISettingsService settingsService)
        {
            _settingsService = settingsService;
            Cookies = new Cookies(httpContextWrapper, settingsService);
        }

        public void SaveMarketingData(string data)
        {
            if (_settingsService.Settings.MarketingCookieEnabled)
            {
                Cookies.WriteCookie(CookieConstants.Marketing, data);
                return;
            }
            
            Cookies.DeleteCookie(CookieConstants.Marketing);
        }
    }
}
