using System.Collections.Generic;
using Pegasus.Domain.ProjectTask;
using Pegasus.Entities.Sorters.ProjectTask;
using Pegasus.Extensions;
using Pegasus.Library.Models;

namespace Pegasus.Models.TaskList
{
    public class IndexViewModel : BaseViewModel
    {
        private readonly IEnumerable<TaskModel> _projectTasks;
        private const int DefaultPageSize = 10;

        public IndexViewModel(IEnumerable<TaskModel> projectTasks)
        {
            _projectTasks = projectTasks;
        }

        public IEnumerable<ProjectModel> Projects { get; set; }
        public ProjectModel Project { get; set; }
        public ISorter Sorter { get; set; } = new ModifiedDescSorter();
        public int TaskFilterId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = DefaultPageSize;

        public IEnumerable<TaskFilter> TaskFilters { get; set; }

        public IEnumerable<TaskModel> ProjectTasks => _projectTasks
            .Filtered(TaskFilterId)
            .Sorted(Sorter)
            .Paginated(Page, PageSize);
    }
}
