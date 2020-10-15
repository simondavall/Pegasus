using System.Collections.Generic;
using System.Linq;
using Pegasus.Entities.Enumerations;

namespace Pegasus.Entities
{
    public class TaskFilter
    {
        public string Name { get; private set; }
        public int Value { get; private set; }

        public static IEnumerable<TaskFilter> GetAllTaskFilters()
        {
            const TaskFilters taskValue = TaskFilters.All;
            return EnumHelper<TaskFilters>.GetValues(taskValue)
                .Select(task => new TaskFilter {
                    Name = EnumHelper<TaskFilters>.GetDisplayValue(task), 
                    Value = (int)task
                }).ToList();
        }
    }
}
