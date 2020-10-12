using System.Collections.Generic;
using Pegasus.Entities;

namespace Pegasus.Models.Home
{
    public class IndexViewModel : BaseViewModel
    {
        private readonly IEnumerable<ProjectTaskExt> _projectTasks;

        public IndexViewModel(IEnumerable<ProjectTaskExt> projectTasks)
        {
            _projectTasks = projectTasks;
        }

        public IEnumerable<Project> Projects { get; set; }
        public Project Project { get; set; }
        public int TaskFilterId { get; set; }

        public IEnumerable<ProjectTaskExt> ProjectTasks => _projectTasks.FilteredProjects(TaskFilterId);
    }

    
}
