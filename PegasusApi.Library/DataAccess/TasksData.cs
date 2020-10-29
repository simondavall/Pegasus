using System.Collections.Generic;
using System.Linq;
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

        public TaskModel GetTask(int id)
        {
            var output = _dataAccess.LoadData<TaskModel, dynamic>("spTasks_Get", new { id }, ConnectionStringName);
            return output.FirstOrDefault();
        }

        public List<TaskModel> GetTasks(int projectId)
        {
            var output = _dataAccess.LoadData<TaskModel, dynamic>("spTasks_GetAllForProject", new { projectId }, ConnectionStringName);
            return output;
        }

        public List<TaskModel> GetAllTasks()
        {
            var output = _dataAccess.LoadData<TaskModel, dynamic>("spTasks_GetAll", new { }, ConnectionStringName);
            return output;
        }

        public List<TaskPriorityModel> GetAllTaskPriorities()
        {
            var output = _dataAccess.LoadData<TaskPriorityModel, dynamic>("spTasks_GetAllTaskPriorities", new { }, ConnectionStringName);
            return output;
        }

        public List<TaskStatusModel> GetAllTaskStatuses()
        {
            var output = _dataAccess.LoadData<TaskStatusModel, dynamic>("spTasks_GetAllTaskStatuses", new { }, ConnectionStringName);
            return output;
        }

        public List<TaskTypeModel> GetAllTaskTypes()
        {
            var output = _dataAccess.LoadData<TaskTypeModel, dynamic>("spTasks_GetAllTaskTypes", new { }, ConnectionStringName);
            return output;
        }

        public void AddTask(TaskModel task)
        {
            var parameters = new
            {
                task.Name, task.Description, task.ProjectId, task.TaskStatusId, 
                task.TaskTypeId, task.TaskPriorityId, task.FixedInRelease
            };

            _dataAccess.SaveData<dynamic>("spTasks_Add", parameters, ConnectionStringName);
        }

        public void UpdateTask(TaskModel task)
        {
            var parameters = new
            {
                task.Id, task.Name, task.Description, task.ProjectId, task.TaskStatusId, 
                task.TaskTypeId, task.TaskPriorityId, task.FixedInRelease
            };

            _dataAccess.SaveData<dynamic>("spTasks_Update", parameters, ConnectionStringName);
        }


    }
}
