using System.Collections.Generic;
using System.Reflection;
using Pegasus.Entities.Enumerations;
using Pegasus.Entities.Profiles;

namespace Pegasus.Entities
{
    /// <summary>
    /// Extends ProjectTask with profile properties needed by Model but not to be stored by EF in db.
    /// </summary>
    public class ProjectTaskExt : ProjectTask
    {
        public ProjectTaskExt(ProjectTask projectTask)
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
                if (TaskPriorityId > 3)
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

        /// <summary>
        /// Convert a list of ProjectTask to a list of ProjectTaskExt 
        /// </summary>
        /// <param name="projectTasks"></param>
        /// <returns></returns>
        public static IEnumerable<ProjectTaskExt> Convert(IEnumerable<ProjectTask> projectTasks)
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
