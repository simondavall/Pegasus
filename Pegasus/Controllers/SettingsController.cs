using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pegasus.Domain;
using Pegasus.Models.Settings;

namespace Pegasus.Controllers
{
    [Authorize(Roles = "PegasusUser")]
    public class SettingsController : Controller
    {
        private readonly ISettingsModel _settingsModel;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SettingsController> _logger;
        private readonly Cookies _cookies;

        public SettingsController(ISettingsModel settingsModel, IConfiguration configuration, ILogger<SettingsController> logger)
        {
            _settingsModel = settingsModel;
            _configuration = configuration;
            _logger = logger;
            _cookies = new Cookies(configuration);
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
