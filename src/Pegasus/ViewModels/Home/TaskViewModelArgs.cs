using System.Collections.Generic;
using Pegasus.Entities;
using Pegasus.Services;

namespace Pegasus.ViewModels.Home
{
    public class TaskViewModelArgs
    {
        public IPegasusData PegasusData { get; set; }
        public ProjectTask ProjectTask { get; set; }
        public Project Project { get; set; }
        public int ExistingStatusId { get; set; } = 0;
        public string NewComment { get; set; } = "";
        public IEnumerable<TaskComment> Comments { get; set; }
    }
}
