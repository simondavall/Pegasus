using System.Collections.Generic;
using System.Linq;
using Pegasus.Entities.Enumerations;
using Pegasus.Entities.Profiles;
using Pegasus.Entities.Sorters.ProjectTask;
using Pegasus.Library.Models;

namespace Pegasus.Extensions
{
    public static class TaskModelExtensions
    {
        public static bool IsClosed(this TaskModel model)
        {
            return model.TaskStatusId == (int) TaskStatusEnum.Completed ||
                   model.TaskStatusId == (int) TaskStatusEnum.Obsolete;
        }

        public static string PriorityIconClass(this TaskModel model)
        {
            if (model.TaskPriorityId > (int) TaskPriorityEnum.Normal)
            {
                if (model.IsClosed()) return "priority-icon-closed";
                return $"priority-icon-{model.ToString()?.ToLower()}";
            }

            return string.Empty;
        }

        public static bool HasParentTask(this TaskModel model)
        {
            return model.ParentTaskId != null;
        }
        
        public static ITaskProfile TaskProfile(this TaskModel model)
        {
            return TaskProfileResolver.GetTaskProfile((TaskStatusEnum)model.TaskStatusId);
        }

        private static IEnumerable<TaskModel> IsNotClosed(this IEnumerable<TaskModel> projectTasks)
        {
            return projectTasks.IsNotCompleted().IsNotObsolete();
        }

        private static IEnumerable<TaskModel> IsNotCompleted(this IEnumerable<TaskModel> projectTasks)
        {
            return projectTasks.Where(pt => pt.TaskStatusId != (int)TaskStatusEnum.Completed);
        }

        private static IEnumerable<TaskModel> IsHighPriority(this IEnumerable<TaskModel> projectTasks)
        {
            return projectTasks.Where(pt => pt.TaskPriorityId >= (int)TaskPriorityEnum.High);
        }

        private static IEnumerable<TaskModel> IsInBacklog(this IEnumerable<TaskModel> projectTasks)
        {
            return projectTasks.Where(pt => pt.TaskStatusId == (int)TaskStatusEnum.Backlog);
        }

        private static IEnumerable<TaskModel> IsNotObsolete(this IEnumerable<TaskModel> projectTasks)
        {
            return projectTasks.Where(pt => pt.TaskStatusId != (int)TaskStatusEnum.Obsolete);
        }

        private static IEnumerable<TaskModel> IsNotInBacklog(this IEnumerable<TaskModel> projectTasks)
        {
            return projectTasks.Where(pt => pt.TaskStatusId != (int)TaskStatusEnum.Backlog);
        }

        private static IEnumerable<TaskModel> IsObsolete(this IEnumerable<TaskModel> projectTasks)
        {
            return projectTasks.Where(pt => pt.TaskStatusId == (int)TaskStatusEnum.Obsolete);
        }

        public static IEnumerable<TaskModel> Sorted(this IEnumerable<TaskModel> projectTasks, ISorter sorter)
        {
            return sorter.Sort(projectTasks);
        }

        internal static IEnumerable<TaskModel> Filtered(this IEnumerable<TaskModel> projectTasks, int taskFilterId)
        {
            switch ((TaskFilters)taskFilterId)
            {
                case TaskFilters.Open:
                    return projectTasks.IsNotClosed().IsNotInBacklog();
                case TaskFilters.Backlog:
                    return projectTasks.IsInBacklog();
                case TaskFilters.HighPriority:
                    return projectTasks.IsHighPriority().IsNotObsolete();
                case TaskFilters.Obsolete:
                    return projectTasks.IsObsolete();
                default:
                    return projectTasks.IsNotObsolete();
            }
        }
    }
}