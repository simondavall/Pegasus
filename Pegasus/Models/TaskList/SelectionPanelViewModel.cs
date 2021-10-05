using System.Collections.Generic;
using Pegasus.Library.Models;
using Pegasus.Services.Models;

namespace Pegasus.Models.TaskList
{
    public class SelectionPanelViewModel
    {
        public int Page { get; set; }
        public int ProjectId { get; set; }
        public IEnumerable<ProjectModel> Projects { get; set; }
        public int TaskFilterId { get; set; }
        public IEnumerable<TaskFilterModel> TaskFilters { get; set; }
    }
}
