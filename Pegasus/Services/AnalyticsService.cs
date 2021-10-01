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

        // create a marketing cookie to simulate a marketing scenario (might actually be a third party tool/plugin)
        private readonly Cookies _cookies;

        public AnalyticsService(IHttpContextAccessor httpContextAccessor, ISettingsService settingsService)
        {
            _httpContextAccessor = httpContextAccessor;
            _cookies = new Cookies(settingsService);
        }

        public void SaveAnalyticsData(string data)
        {
            _cookies.WriteCookie(_httpContextAccessor.HttpContext.Response, CookieConstants.Analytics, data);
        }
    }
}
