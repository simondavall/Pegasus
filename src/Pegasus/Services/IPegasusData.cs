using System.Collections.Generic;
using System.Threading.Tasks;
using Pegasus.Entities;
using TaskStatus = Pegasus.Entities.TaskStatus;

namespace Pegasus.Services
{
    public interface IPegasusData
    {
        IEnumerable<Project> GetAllProjects();
        IEnumerable<ProjectTask> GetAllTasks();
        IEnumerable<TaskComment> GetAllComments();

        Project GetProject(int id);
        Project AddProject(Project project);

        ProjectTask GetTask(int id);
        IEnumerable<ProjectTask> GetTasks(int projectId);
        void AddTask(ProjectTask projectTask);

        TaskComment GetComment(int id);
        IEnumerable<TaskComment> GetComments(int taskId);
        TaskComment AddComment(TaskComment taskComment);

        IEnumerable<TaskStatus> GetAllTaskStatuses();
        TaskStatus AddTaskStatus(TaskStatus taskStatus);

        IEnumerable<TaskType> GetAllTaskTypes();
        TaskType AddTaskType(TaskType taskType);

        TaskStatusHistory AddStatusHistory(TaskStatusHistory statusHistory);
    }
}
