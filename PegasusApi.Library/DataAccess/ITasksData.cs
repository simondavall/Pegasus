using System.Collections.Generic;
using PegasusApi.Library.Models;

namespace PegasusApi.Library.DataAccess
{
    public interface ITasksData
    {
        TaskModel GetTask(int id);
        List<TaskModel> GetTasks(int projectId);
        List<TaskModel> GetAllTasks();
        List<TaskPriorityModel> GetAllTaskPriorities();
        List<TaskStatusModel> GetAllTaskStatuses();
        List<TaskTypeModel> GetAllTaskTypes();
        void AddTask(TaskModel task);
        void UpdateTask(TaskModel task);
    }
}