using System.Collections.Generic;
using System.Reflection;
using Pegasus.Entities.Enumerations;
using Pegasus.Entities.Profiles;
using Pegasus.Library.Models;

namespace Pegasus.Entities
{
    /// <summary>
    /// Extends TaskModel with profile properties needed by Model but not to be stored by EF in db.
    /// </summary>
    public class ProjectTaskExt : TaskModel
    {
        public ProjectTaskExt(TaskModel projectTask)
        {
            var properties = projectTask.GetType().GetRuntimeProperties();
            foreach (var property in properties)
            {
                if (property.CanWrite)
                {
                    property.SetValue(this, property.GetValue(projectTask));
                }
            }
            TaskProfile = TaskProfileResolver.SetTaskProfile((TaskStatusEnum)TaskStatusId);
        }

        public ITaskProfile TaskProfile { get; set; }

        public string PriorityIconClass
        {
            get
            {
                if (TaskPriorityId > (int)TaskPriorityEnum.Normal)
                {
                    if (IsClosed)
                    {
                        return "priority-icon-closed";
                    }
                    return $"priority-icon-{TaskPriorityId.ToString().ToLower()}";
                }
                return string.Empty;
            }
        }

        public bool IsClosed
        {
            get { return TaskStatusId == (int)TaskStatusEnum.Completed || TaskStatusId == (int)TaskStatusEnum.Obsolete; }
        }

        /// <summary>
        /// Convert a list of ProjectTask to a list of ProjectTaskExt 
        /// </summary>
        /// <param name="projectTasks"></param>
        /// <returns></returns>
        public static IEnumerable<ProjectTaskExt> Convert(IEnumerable<TaskModel> projectTasks)
        {
            var result = new List<ProjectTaskExt>();
            foreach (var projectTask in projectTasks)
            {
                result.Add(new ProjectTaskExt(projectTask));
            }
            return result;
        }
    }
}
