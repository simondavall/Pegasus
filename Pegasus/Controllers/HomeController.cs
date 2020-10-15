using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Pegasus.Models;
using Pegasus.Services;

namespace Pegasus.Controllers
{
    public class HomeController : TaskListController
    {
        public HomeController(IPegasusData pegasusData, IConfiguration configuration) : base(pegasusData, configuration)
        {

        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            var model = new BaseViewModel { ProjectId = 0 };

            return View(model);
        }
    }
}
