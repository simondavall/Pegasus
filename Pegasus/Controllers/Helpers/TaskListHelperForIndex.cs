using System.Threading.Tasks;
using Pegasus.Library.Models;
using Pegasus.Models.TaskList;

namespace Pegasus.Controllers.Helpers
{
    public class TaskListHelperForIndex : TaskListHelperBase
    {
        public TaskListHelperForIndex(TaskListHelperArgs args) : base(args)
        { }
        
        internal async Task<IndexViewModel> GetIndexViewModel()
        {
            var taskFilterId = SettingsService.GetSetting<int>(nameof(SettingsService.Settings.TaskFilterId));
            var projectId = SettingsService.GetSetting<int>(nameof(SettingsService.Settings.ProjectId));

            var project = await ProjectsEndpoint.GetProject(projectId) ?? new ProjectModel {Id = 0, Name = "All"};
            var projectTasks = project.Id > 0
                ? await TasksEndpoint.GetTasks(project.Id)
                : await TasksEndpoint.GetAllTasks();

            return new IndexViewModel(projectTasks, taskFilterId, SettingsService.Settings)
            {
                ProjectId = project.Id,
                Page = GetPage(),
                PageSize = SettingsService.Settings.PageSize,
                Projects = await ProjectsEndpoint.GetAllProjects(),
                TaskFilters = TaskFilterService.GetTaskFilters(),
                Project = project
            };
        }
        
        private int GetPage()
        {
            const int defaultPageNo = 1;
            var qsPage = Controller.Request.Query["page"];
            return int.TryParse(qsPage, out var pageNo) ? pageNo : defaultPageNo;
        }
    }
}