using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PegasusApi.Library.Models;

namespace PegasusApi.Library.DataAccess
{
    public class TasksData : ITasksData
    {
        private readonly IDataAccess _dataAccess;
        private const string ConnectionStringName = "Pegasus";

        public TasksData(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task<TaskModel> GetTask(int id)
        {
            var output = await _dataAccess.LoadDataAsync<TaskModel, dynamic>("spTasks_Get", new { id }, ConnectionStringName);
            return output.FirstOrDefault();
        }

        public async Task<List<TaskModel>> GetTasks(int projectId)
        {
            var output = await _dataAccess.LoadDataAsync<TaskModel, dynamic>("spTasks_GetAllForProject", new { projectId }, ConnectionStringName);
            return output;
        }

        public async Task<List<TaskModel>> GetSubTasks(int taskId)
        {
            var output = await _dataAccess.LoadDataAsync<TaskModel, dynamic>("spTasks_GetSubTasks", new { taskId }, ConnectionStringName);
            return output;
        }
        
        public async Task<List<TaskModel>> GetAllTasks()
        {
            var output = await _dataAccess.LoadDataAsync<TaskModel, dynamic>("spTasks_GetAll", new { }, ConnectionStringName);
            return output;
        }

        public async Task<List<TaskPriorityModel>> GetAllTaskPriorities()
        {
            var output = await _dataAccess.LoadDataAsync<TaskPriorityModel, dynamic>("spTasks_GetAllTaskPriorities", new { }, ConnectionStringName);
            return output;
        }

        public async Task<List<TaskStatusModel>> GetAllTaskStatuses()
        {
            var output = await _dataAccess.LoadDataAsync<TaskStatusModel, dynamic>("spTasks_GetAllTaskStatuses", new { }, ConnectionStringName);
            return output;
        }

        public async Task<List<TaskTypeModel>> GetAllTaskTypes()
        {
            var output = await _dataAccess.LoadDataAsync<TaskTypeModel, dynamic>("spTasks_GetAllTaskTypes", new { }, ConnectionStringName);
            return output;
        }

        public async Task<int> AddTask(TaskModel task)
        {
            var parameters = new
            {
                task.Name, task.Description, task.ProjectId, task.TaskStatusId, 
                task.TaskTypeId, task.TaskPriorityId, task.FixedInRelease, task.UserId, task.ParentTaskId
            };

            return await _dataAccess.ExecuteScalarAsync<int, dynamic>("spTasks_Add", parameters, ConnectionStringName);
        }

        public async Task UpdateTask(TaskModel task)
        {
            var parameters = new
            {
                task.Id, task.Name, task.Description, task.ProjectId, task.TaskStatusId, 
                task.TaskTypeId, task.TaskPriorityId, task.FixedInRelease, task.UserId
            };

            await _dataAccess.SaveDataAsync<dynamic>("spTasks_Update", parameters, ConnectionStringName);
        }

        public async Task AddTaskStatus(TaskStatusModel taskStatus)
        {
            var parameters = new
            {
                taskStatus.Name,
                taskStatus.DisplayOrder
            };

            await _dataAccess.SaveDataAsync<dynamic>("spTasks_AddTaskStatus", parameters, ConnectionStringName);
        }

        public async Task AddTaskType(TaskTypeModel taskType)
        {
            var parameters = new
            {
                taskType.Name,
                taskType.DisplayOrder
            };

            await _dataAccess.SaveDataAsync<dynamic>("spTasks_AddTaskType", parameters, ConnectionStringName);
        }

        public async Task AddTaskPriority(TaskPriorityModel taskPriority)
        {
            var parameters = new
            {
                taskPriority.Name,
                taskPriority.DisplayOrder
            };

            await _dataAccess.SaveDataAsync<dynamic>("spTasks_AddTaskPriority", parameters, ConnectionStringName);
        }

        public async Task<List<TaskStatusHistoryModel>> GetStatusHistory(int taskId)
        {
            var output = await _dataAccess.LoadDataAsync<TaskStatusHistoryModel, dynamic>("spTasks_GetStatusHistory", new { taskId }, ConnectionStringName);
            return output;
        }
    }
}
