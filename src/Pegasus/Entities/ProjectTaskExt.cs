using System.Collections.Generic;
using System.Reflection;
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
            var fields = projectTask.GetType().GetRuntimeProperties();
            foreach (var field in fields)
            {
                field.SetValue(this, field.GetValue(projectTask));
            }
            SetTaskProfile(TaskStatusId);
        }
        public ITaskProfile TaskProfile { get; set; }

        private void SetTaskProfile(int taskStatusId)
        {
            switch (taskStatusId)
            {
                case 3:
                    TaskProfile = new CompletedTaskProfile();
                    break;
                case 2:
                    TaskProfile = new InProgressTaskProfile();
                    break;
                default:
                    TaskProfile = new DefaultTaskProfile();
                    break;
            }

        }

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
