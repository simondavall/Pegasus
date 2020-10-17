using Microsoft.AspNetCore.Mvc;
using Pegasus.Models;

namespace Pegasus.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Message"] = "For more information, please contact:";
            var model = new BaseViewModel { ProjectId = 0 };

            return View(model);
        }
    }


}
