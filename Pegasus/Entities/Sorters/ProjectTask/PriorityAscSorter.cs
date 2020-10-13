using System.Collections.Generic;
using System.Linq;

namespace Pegasus.Entities.Sorters.ProjectTask
{
    public class PriorityAscSorter : ISorter
    {
        public IOrderedEnumerable<ProjectTaskExt> Sort(IEnumerable<ProjectTaskExt> projectTaskExts)
        {
            return projectTaskExts.OrderBy(pt => pt.TaskPriorityId);
        }
    }
}
