using Microsoft.AspNetCore.Http;
using Pegasus.Domain;
using Pegasus.Library.JwtAuthentication.Constants;

namespace Pegasus.Services
{
    public interface IAnalyticsService
    {
        void SaveAnalyticsData(string data);
    }

    public class AnalyticsService : IAnalyticsService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISettingsService _settingsService;

        // create a marketing cookie to simulate a marketing scenario (might actually be a third party tool/plugin)
        protected ICookies Cookies;

        public AnalyticsService(IHttpContextAccessor httpContextAccessor, ISettingsService settingsService)
        {
            _httpContextAccessor = httpContextAccessor;
            _settingsService = settingsService;
            Cookies = new Cookies(httpContextAccessor, settingsService);
        }

        public void SaveAnalyticsData(string data)
        {
            if (_settingsService.Settings.AnalyticsCookieEnabled)
            {
                Cookies.WriteCookie(_httpContextAccessor.HttpContext.Response, CookieConstants.Analytics, data);
                return;
            }
            
            Cookies.DeleteCookie(_httpContextAccessor.HttpContext.Response, CookieConstants.Analytics);
        }
    }
}
