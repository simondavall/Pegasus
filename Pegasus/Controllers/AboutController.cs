using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Pegasus.Models;

namespace Pegasus.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Message"] = "For more information, please contact:";
            ViewData["Version"] = Assembly.GetEntryAssembly()?.GetName().Version;
            var model = new BaseViewModel { ProjectId = 0 };

            return View(model);
        }
    }
}
