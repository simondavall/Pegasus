using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Pegasus.Domain.ProjectTask;
using Pegasus.Library.Api;
using Pegasus.Models.Settings;

namespace Pegasus.Controllers
{
    [Authorize(Roles = "PegasusUser")]
    public class HomeController : TaskListController
    {
        public HomeController(IConfiguration configuration, ITaskFilterService taskFilterService, 
            IProjectsEndpoint projectsEndpoint, ITasksEndpoint tasksEndpoint, ICommentsEndpoint commentsEndpoint, ISettingsModel settingsModel) 
            : base(configuration, taskFilterService, projectsEndpoint, tasksEndpoint, commentsEndpoint, settingsModel)
        { }
    }
}
