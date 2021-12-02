using System.Collections.Generic;
using System.Threading.Tasks;
using Pegasus.Library.Models;

namespace Pegasus.Library.Api
{
    public interface ITasksEndpoint
    {
        Task<int> AddTask(TaskModel taskModel);
        Task<List<TaskModel>> GetAllTasks();
        Task<List<TaskPriorityModel>> GetAllTaskPriorities();
        Task<List<TaskStatusModel>> GetAllTaskStatuses();
        Task<List<TaskTypeModel>> GetAllTaskTypes();
        Task<List<TaskModel>> GetSubTasks(int taskId);
        Task<TaskModel> GetTask(int taskId);
        Task<TaskModel> GetTaskByRef(string taskRef);
        Task<List<TaskModel>> GetTasks(int projectId);
        Task UpdateTask(TaskModel taskModel);
    }

    public class TasksEndpoint : ITasksEndpoint
    {
        private readonly IApiHelper _apiHelper;

        public TasksEndpoint(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<int> AddTask(TaskModel taskModel)
        {
            return await _apiHelper.PostAsync<TaskModel, int>(taskModel, "api/Task/AddTask");
        }

        public async Task<List<TaskModel>> GetAllTasks()
        {
            return await _apiHelper.GetListFromUri<TaskModel>("api/Task/GetAllTasks");
        }
        
        public async Task<List<TaskPriorityModel>> GetAllTaskPriorities()
        {
            return await _apiHelper.GetListFromUri<TaskPriorityModel>("api/Task/GetAllTaskPriorities");
        }

        public async Task<List<TaskStatusModel>> GetAllTaskStatuses()
        {
            return await _apiHelper.GetListFromUri<TaskStatusModel>("api/Task/GetAllTaskStatuses");
        }

        public async Task<List<TaskTypeModel>> GetAllTaskTypes()
        {
            return await _apiHelper.GetListFromUri<TaskTypeModel>("api/Task/GetAllTaskTypes");
        }

        public async Task<List<TaskModel>> GetSubTasks(int taskId)
        {
            return await _apiHelper.GetListFromUri<TaskModel>($"api/Task/GetSubTasks/{taskId}");
        }
        
        public async Task<TaskModel> GetTask(int taskId)
        {
            return await _apiHelper.GetFromUri<TaskModel>($"api/Task/GetTask/{taskId}");
        }

        public async Task<TaskModel> GetTaskByRef(string taskRef)
        {
            return await _apiHelper.GetFromUri<TaskModel>($"api/Task/GetTaskByRef/{taskRef}");
        }

        public async Task<List<TaskModel>> GetTasks(int projectId)
        {
            return await _apiHelper.GetListFromUri<TaskModel>($"api/Task/GetTasks/{projectId}");
        }

        public async Task UpdateTask(TaskModel taskModel)
        {
            await _apiHelper.PostAsync(taskModel, "api/Task/UpdateTask");
        }
    }
}