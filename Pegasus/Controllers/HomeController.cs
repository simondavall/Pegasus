using Microsoft.Extensions.Configuration;
using Pegasus.Domain.ProjectTask;
using Pegasus.Library.Api;

namespace Pegasus.Controllers
{
    public class HomeController : TaskListController
    {
        public HomeController(IConfiguration configuration, ITaskFilterService taskFilterService, 
            IProjectsEndpoint projectsEndpoint, ITasksEndpoint tasksEndpoint, ICommentsEndpoint commentsEndpoint) 
            : base(configuration, taskFilterService, projectsEndpoint, tasksEndpoint, commentsEndpoint)
        { }
    }
}
