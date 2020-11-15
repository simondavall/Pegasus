using Microsoft.AspNetCore.Authorization;
using Pegasus.Library.Api;
using Pegasus.Models.Settings;
using Pegasus.Services;

namespace Pegasus.Controllers
{
    [Authorize(Roles = "PegasusUser")]
    public class HomeController : TaskListController
    {
        public HomeController(ITaskFilterService taskFilterService, 
            IProjectsEndpoint projectsEndpoint, ITasksEndpoint tasksEndpoint, ICommentsEndpoint commentsEndpoint, ISettingsModel settingsModel) 
            : base(taskFilterService, projectsEndpoint, tasksEndpoint, commentsEndpoint, settingsModel)
        { }
    }
}
