using System.Collections.Generic;
using System.Linq;
using Pegasus.Library.Models;

namespace Pegasus.Entities.Sorters.ProjectTask
{
    public static class ProjectTaskSortExt
    {
        public static IOrderedEnumerable<TaskModel> Sorted(this IEnumerable<TaskModel> projectTasks, ISorter sorter)
        {
            return sorter.Sort(projectTasks);
        }
    }
}
