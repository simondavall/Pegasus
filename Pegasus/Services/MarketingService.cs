using Microsoft.AspNetCore.Http;
using Pegasus.Domain;
using Pegasus.Library.JwtAuthentication.Constants;

namespace Pegasus.Services
{
    public interface IMarketingService
    {
        void SaveMarketingData(string data);
    }

    public class MarketingService : IMarketingService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISettingsService _settingsService;

        // create a marketing cookie to simulate a marketing scenario (might actually be a third party tool/plugin)
        protected ICookies Cookies;

        public MarketingService(IHttpContextAccessor httpContextAccessor, ISettingsService settingsService)
        {
            _httpContextAccessor = httpContextAccessor;
            _settingsService = settingsService;
            Cookies = new Cookies(httpContextAccessor, settingsService);
        }

        public void SaveMarketingData(string data)
        {
            if (_settingsService.Settings.MarketingCookieEnabled)
            {
                Cookies.WriteCookie(_httpContextAccessor.HttpContext.Response, CookieConstants.Marketing, data);
                return;
            }
            
            Cookies.DeleteCookie(_httpContextAccessor.HttpContext.Response, CookieConstants.Marketing);
        }
    }
}
