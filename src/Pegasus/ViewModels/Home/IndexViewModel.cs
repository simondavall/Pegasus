using System.Collections.Generic;
using Pegasus.Entities;

namespace Pegasus.ViewModels.Home
{
    public class IndexViewModel : BaseViewModel
    {
        public IEnumerable<ProjectTaskExt> ProjectTasks { get; set; }
        public IEnumerable<Project> Projects { get; set; }
        public Project Project { get; set; }
        public int TaskFilterId { get; set; }
    }
}
