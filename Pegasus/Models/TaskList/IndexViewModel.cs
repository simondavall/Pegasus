using System.Collections.Generic;
using Pegasus.Domain.ProjectTask;
using Pegasus.Entities;
using Pegasus.Entities.Sorters.ProjectTask;
using Pegasus.Library.Models;

namespace Pegasus.Models.TaskList
{
    public class IndexViewModel : BaseViewModel
    {
        private readonly IEnumerable<ProjectTaskExt> _projectTasks;

        public IndexViewModel(IEnumerable<ProjectTaskExt> projectTasks)
        {
            _projectTasks = projectTasks;
        }

        public IEnumerable<ProjectModel> Projects { get; set; }
        public ProjectModel Project { get; set; }
        public ISorter Sorter { get; set; } = new ModifiedDescSorter();
        public int TaskFilterId { get; set; }

        public IEnumerable<TaskFilter> TaskFilters { get; set; }

        public IEnumerable<ProjectTaskExt> ProjectTasks => _projectTasks.Filtered(TaskFilterId).Sorted(Sorter);
    }
}
