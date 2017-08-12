using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pegasus.Entities;

namespace Pegasus.ViewModels.Home
{
    public class TaskViewModel : BaseViewModel
    {
        public ProjectTask ProjectTask { get; set; }
        public IEnumerable<SelectListItem> TaskStatuses { get; set; }
        public IEnumerable<SelectListItem> TaskTypes { get; set; }
        public IEnumerable<SelectListItem> TaskPriorities { get; set; }
        public Project Project { get; set; }
        public string FixedInRelease { get; set; }
        public string Action { get; set; }
        public string ButtonText { get; set; }

        public int ExistingTaskStatus { get; set; }
    }
}
