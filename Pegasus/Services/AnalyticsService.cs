using Pegasus.Domain;
using Pegasus.Library.JwtAuthentication.Constants;
using Pegasus.Library.Services.Http;

namespace Pegasus.Services
{
    public interface IAnalyticsService
    {
        void SaveAnalyticsData(string data);
    }

    public class AnalyticsService : IAnalyticsService
    {
        private readonly IHttpContextWrapper _httpContext;
        private readonly ISettingsService _settingsService;

        // create a marketing cookie to simulate a marketing scenario (might actually be a third party tool/plugin)
        protected ICookies Cookies;

        public AnalyticsService(IHttpContextWrapper httpContextWrapper, ISettingsService settingsService)
        {
            _httpContext = httpContextWrapper;
            _settingsService = settingsService;
            Cookies = new Cookies(_httpContext, settingsService);
        }

        public void SaveAnalyticsData(string data)
        {
            if (_settingsService.Settings.AnalyticsCookieEnabled)
            {
                Cookies.WriteCookie(_httpContext.Response, CookieConstants.Analytics, data);
                return;
            }
            
            Cookies.DeleteCookie(_httpContext.Response, CookieConstants.Analytics);
        }
    }
}
