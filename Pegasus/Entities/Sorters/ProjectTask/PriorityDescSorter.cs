using System.Collections.Generic;
using System.Linq;

namespace Pegasus.Entities.Sorters.ProjectTask
{
    public class PriorityDescSorter : ISorter
    {
        public IOrderedEnumerable<ProjectTaskExt> Sort(IEnumerable<ProjectTaskExt> projectTaskExts)
        {
            return projectTaskExts.OrderByDescending(pt => pt.TaskPriorityId);
        }
    }
}
