using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Pegasus.Services;
using Pegasus.Services.Models;

namespace Pegasus.Controllers
{
    public class CookiePolicyController : Controller
    {
        private readonly ISettingsService _settingsService;

        public CookiePolicyController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveSelected(SettingsModel model)
        {
            _settingsService.Settings.CookiePolicyAccepted = true;
            _settingsService.Settings.MarketingCookieEnabled = model.MarketingCookieEnabled;
            _settingsService.Settings.AnalyticsCookieEnabled = model.AnalyticsCookieEnabled;
            _settingsService.SaveSettings();

            return PartialView("_CookiePolicy", model);
        }
    }
}
