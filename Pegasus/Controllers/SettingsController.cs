using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pegasus.Models.Settings;

namespace Pegasus.Controllers
{
    [Authorize(Roles = "PegasusUser")]
    public class SettingsController : Controller
    {
        private readonly ISettingsModel _settingsModel;

        public SettingsController(ISettingsModel settingsModel)
        {
            _settingsModel = settingsModel;
        }

        public IActionResult Index()
        {
            return View(_settingsModel);
        }

        [HttpPost]
        public IActionResult SaveSettings([Bind("PageSize,PaginationEnabled")] SettingsModel settings, string returnUrl)
        {
            _settingsModel.PageSize = settings.PageSize;
            _settingsModel.PaginationEnabled = settings.PaginationEnabled;
            _settingsModel.SaveSettings();

            return Redirect(returnUrl);
        }
    }
}
