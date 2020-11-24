using Microsoft.AspNetCore.Authorization;
using Pegasus.Entities.Attributes;
using Pegasus.Library.Api;
using Pegasus.Services;

namespace Pegasus.Controllers
{
    [Authorize(Roles = "PegasusUser")]
    [Require2Fa]
    public class HomeController : TaskListController
    {
        public HomeController(ITaskFilterService taskFilterService, 
            IProjectsEndpoint projectsEndpoint, ITasksEndpoint tasksEndpoint, ICommentsEndpoint commentsEndpoint, ISettingsService settingsService) 
            : base(taskFilterService, projectsEndpoint, tasksEndpoint, commentsEndpoint, settingsService)
        { }
    }
}
