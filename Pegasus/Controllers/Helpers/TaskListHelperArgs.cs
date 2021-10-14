using Microsoft.AspNetCore.Mvc;
using Pegasus.Library.Api;
using Pegasus.Services;

namespace Pegasus.Controllers.Helpers
{
    public class TaskListHelperArgs
    {
        public TaskListHelperArgs(Controller controller, ITasksEndpoint tasksEndpoint, ISettingsService settingsService, 
            IProjectsEndpoint projectsEndpoint, ICommentsEndpoint commentsEndpoint, ITaskFilterService taskFilterService)
        {
            Controller = controller;
            TasksEndpoint = tasksEndpoint;
            SettingsService = settingsService;
            ProjectsEndpoint = projectsEndpoint;
            CommentsEndpoint = commentsEndpoint;
            TaskFilterService = taskFilterService;
        }
        public Controller Controller { get; }
        public ITasksEndpoint TasksEndpoint { get; }
        public ISettingsService SettingsService { get; }
        public IProjectsEndpoint ProjectsEndpoint { get; }
        public ICommentsEndpoint CommentsEndpoint { get; }
        public ITaskFilterService TaskFilterService { get; }
    }
}