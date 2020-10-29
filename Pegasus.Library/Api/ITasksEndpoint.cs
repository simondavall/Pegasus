using System.Collections.Generic;
using System.Threading.Tasks;
using Pegasus.Library.Models;

namespace Pegasus.Library.Api
{
    public interface ITasksEndpoint
    {
        Task<List<TaskModel>> GetAllTasks();
        Task<List<TaskModel>> GetTasks(int projectId);
        Task<TaskModel> GetTask(int taskId);
        Task<List<TaskPriorityModel>> GetAllTaskPriorities();
        Task<List<TaskStatusModel>> GetAllTaskStatuses();
        Task<List<TaskTypeModel>> GetAllTaskTypes();

        Task AddTask(TaskModel taskModel);
        Task UpdateTask(TaskModel taskModel);
    }
}