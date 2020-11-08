using System.Collections.Generic;
using System.Linq;
using Pegasus.Library.Models;

namespace Pegasus.Entities.Sorters.ProjectTask
{
    public class PriorityDescSorter : ISorter
    {
        public IOrderedEnumerable<TaskModel> Sort(IEnumerable<TaskModel> projectTasks)
        {
            return projectTasks.OrderByDescending(pt => pt.TaskPriorityId);
        }
    }
}
