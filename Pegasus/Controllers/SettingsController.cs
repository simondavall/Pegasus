using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pegasus.Services;

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
        public IActionResult SaveSettings([Bind("PageSize,PaginationEnabled,CommentSortOrder")] SettingsService settings, string returnUrl)
        {
            _settings.CommentSortOrder = settings.CommentSortOrder;
            _settings.PageSize = settings.PageSize;
            _settings.PaginationEnabled = settings.PaginationEnabled;
            _settings.SaveSettings();

            return Redirect(returnUrl);
        }
    }
}
