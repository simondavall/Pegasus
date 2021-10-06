using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pegasus.Services;
using Pegasus.Services.Models;

namespace Pegasus.Controllers
{
    [Authorize(Roles = "PegasusUser")]
    public class SettingsController : Controller
    {
        private readonly ISettingsService _settings;

        public SettingsController(ISettingsService settings)
        {
            _settings = settings;
        }

        public IActionResult Index()
        {
            return View(_settings);
        }

        [HttpPost]
        public IActionResult SaveSettings([Bind("PageSize,PaginationEnabled,CommentSortOrder")] SettingsModel settings, string returnUrl)
        {
            _settings.Settings.CommentSortOrder = settings.CommentSortOrder;
            _settings.Settings.PageSize = settings.PageSize;
            _settings.Settings.PaginationEnabled = settings.PaginationEnabled;
            _settings.SaveSettings();

            return Redirect(returnUrl);
        }
    }
}
