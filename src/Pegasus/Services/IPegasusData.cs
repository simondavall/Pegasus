using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pegasus.Entities;
using TaskStatus = Pegasus.Entities.TaskStatus;

namespace Pegasus.Services
{
    public interface IPegasusData
    {
        IQueryable<Project> GetAllProjects();
        Project GetProject(int id);
        Task<Project> GetProjectAsync(int? id);
        void AddProject(Project project);
        Task AddProjectAsync(Project project);
        void UpdateProject(Project project);
        Task UpdateProjectAsync(Project project);
        Task DeleteProjectAsync(Project project);

        IEnumerable<ProjectTask> GetAllTasks();
        ProjectTask GetTask(int id);
        IEnumerable<ProjectTask> GetTasks(int projectId);
        void AddTask(ProjectTask projectTask);
        void UpdateTask(ProjectTask projectTask, int existingTaskStatus);

        Task<string> GetNextTaskRef(int projectId, string projectPrefix);
        void AddTaskIndexer(ProjectTaskIndexer projectTaskIndexer);
        Task AddTaskIndexerAsync(ProjectTaskIndexer projectTaskIndexer);

        TaskComment GetComment(int id);
        IEnumerable<TaskComment> GetComments(int taskId);
        void AddComment(TaskComment taskComment);
        void UpdateComment(TaskComment taskComment);
        void UpdateComments(IEnumerable<TaskComment> taskComments);

        IEnumerable<TaskStatus> GetAllTaskStatuses();
        void AddTaskStatus(TaskStatus taskStatus);

        IEnumerable<TaskType> GetAllTaskTypes();
        void AddTaskType(TaskType taskType);

        IEnumerable<TaskPriority> GetAllTaskPriorities();
        void AddTaskPriority(TaskPriority taskPriority);
    }
}
