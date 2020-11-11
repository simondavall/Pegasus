using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pegasus.Models.Settings;

namespace Pegasus.Controllers
{
    [Authorize(Roles = "PegasusUser")]
    public class SettingsController : Controller
    {
        public IActionResult Index()
        {
            var settingsViewModel = new SettingsModel();
            settingsViewModel.Title = "Settings";
            return View(settingsViewModel);
        }

        [HttpPost]
        public IActionResult SaveSettings(string returnUrl)
        {
            //TODO Implement save settings
            return Redirect(returnUrl);
        }
    }
}
