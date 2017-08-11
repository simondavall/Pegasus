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
        public Project Project { get; set; }

        public int ExistingTaskStatus { get; set; }
    }
}
