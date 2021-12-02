using System.Collections.Generic;
using System.Threading.Tasks;
using PegasusApi.Library.Models;

namespace PegasusApi.Library.DataAccess
{
    public interface ITasksData
    {
        Task<TaskModel> GetTask(int id);
        Task<TaskModel> GetTaskByRef(string taskRef);
        Task<List<TaskModel>> GetTasks(int projectId);
        Task<List<TaskModel>> GetAllTasks();
        Task<List<TaskModel>> GetSubTasks(int taskId);
        Task<List<TaskPriorityModel>> GetAllTaskPriorities();
        Task<List<TaskStatusModel>> GetAllTaskStatuses();
        Task<List<TaskTypeModel>> GetAllTaskTypes();
        Task<int> AddTask(TaskModel task);
        Task UpdateTask(TaskModel task);
        Task AddTaskStatus(TaskStatusModel taskStatus);
        Task AddTaskType(TaskTypeModel taskType);
        Task AddTaskPriority(TaskPriorityModel taskPriority);
        Task<List<TaskStatusHistoryModel>> GetStatusHistory(int taskId);
    }
}