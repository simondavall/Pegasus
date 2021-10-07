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
        private readonly IHttpContextWrapper _httpContext;
        private readonly ISettingsService _settingsService;

        // create a marketing cookie to simulate a marketing scenario (might actually be a third party tool/plugin)
        protected ICookies Cookies;

        public MarketingService(IHttpContextWrapper httpContextWrapper, ISettingsService settingsService)
        {
            _httpContext = httpContextWrapper;
            _settingsService = settingsService;
            Cookies = new Cookies(_httpContext, settingsService);
        }

        public void SaveMarketingData(string data)
        {
            if (_settingsService.Settings.MarketingCookieEnabled)
            {
                Cookies.WriteCookie(_httpContext.Response, CookieConstants.Marketing, data);
                return;
            }
            
            Cookies.DeleteCookie(_httpContext.Response, CookieConstants.Marketing);
        }
    }
}
