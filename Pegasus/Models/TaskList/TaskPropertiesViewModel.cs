using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pegasus.Library.Models;

namespace Pegasus.Models.TaskList
{
    public class TaskPropertiesViewModel
    {
        public TaskModel ProjectTask { get; set; }
        public IEnumerable<SelectListItem> TaskPriorities { get; set; }
        public IEnumerable<SelectListItem> TaskStatuses { get; set; }
        public IEnumerable<SelectListItem> TaskTypes { get; set; }
    }
}
