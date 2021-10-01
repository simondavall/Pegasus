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

        // create a marketing cookie to simulate a marketing scenario (might actually be a third party tool/plugin)
        private readonly Cookies _cookies;

        public MarketingService(IHttpContextAccessor httpContextAccessor, ISettingsService settingsService)
        {
            _httpContextAccessor = httpContextAccessor;
            _cookies = new Cookies(settingsService);
        }

        public void SaveMarketingData(string data)
        {
            _cookies.WriteCookie(_httpContextAccessor.HttpContext.Response, CookieConstants.Marketing, data);
        }
    }
}
