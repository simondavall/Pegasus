using System.Diagnostics;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PegasusApi.Models;

namespace PegasusApi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        //private readonly RoleManager<IdentityRole> _roleManager;
        //private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger
            //, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager
            )
        {
            _logger = logger;
            //_roleManager = roleManager;
            //_userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            // List roles
            //var roles = new[] { "Admin", "Manager", "Reports", "Projects" };
            //foreach (var role in roles)
            //{
            //    var roleExist = await _roleManager.RoleExistsAsync(role);
            //    if (!roleExist)
            //    {
            //        await _roleManager.CreateAsync(new IdentityRole(role));
            //    }
            //}

            //var user = await _userManager.FindByEmailAsync("simon.davall@gmail.com");

            //if (user != null)
            //{
            //    await _userManager.AddToRoleAsync(user, "Admin");
            //    await _userManager.AddToRoleAsync(user, "Projects");
            //}

            _logger.LogInformation("Viewed provacy page.");

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
