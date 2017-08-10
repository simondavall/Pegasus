using System.Collections.Generic;
using Pegasus.Entities;

namespace Pegasus.Services
{
    public interface IPegasusData
    {
        IEnumerable<Project> GetAllProjects();
        IEnumerable<ProjectTask> GetAllTasks();
        IEnumerable<TaskComment> GetAllComments();

        Project GeProject(int id);
        Project AddProject(Project project);

        ProjectTask GetTask(int id);
        IEnumerable<ProjectTask> GetTasks(int projectId);
        ProjectTask AddTask(ProjectTask projectTask);

        TaskComment GetComment(int id);
        IEnumerable<TaskComment> GetComments(int taskId);
        TaskComment AddComment(TaskComment taskComment);

        TaskStatus AddTaskStatus(TaskStatus taskStatus);
        TaskType AddTaskType(TaskType taskType);

        TaskStatusHistory AddStatusHistory(TaskStatusHistory statusHistory);
    }
}
