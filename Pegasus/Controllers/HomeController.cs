using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Pegasus.Domain.ProjectTask;
using Pegasus.Library.Api;
using Pegasus.Models;

namespace Pegasus.Controllers
{
    public class HomeController : TaskListController
    {
        public HomeController(IConfiguration configuration, ITaskFilterService taskFilterService, 
            IProjectsEndpoint projectsEndpoint, ITasksEndpoint tasksEndpoint, ICommentsEndpoint commentsEndpoint) 
            : base(configuration, taskFilterService, projectsEndpoint, tasksEndpoint, commentsEndpoint)
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
