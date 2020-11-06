using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PegasusApi.Library.DataAccess;
using PegasusApi.Library.Models;

namespace PegasusApi.Controllers
{
    [Authorize(Roles = "PegasusUser")]
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITasksData _taskData;

        public TaskController(ITasksData taskData)
        {
            _taskData = taskData;
        }

        [Route("GetTask/{taskId}")]
        [HttpGet]
        public TaskModel GetTask(int taskId)
        {
            return _taskData.GetTask(taskId);
        }

        [Route("GetTasks/{projectId}")]
        [HttpGet]
        public IEnumerable<TaskModel> GetTasks(int projectId)
        {
            return _taskData.GetTasks(projectId);
        }

        [Route("GetAllTasks")]
        [HttpGet]
        public IEnumerable<TaskModel> GetAllTasks()
        {
            return _taskData.GetAllTasks();
        }

        [Route("AddTask")]
        [HttpPost]
        public void AddTask(TaskModel task)
        {
            _taskData.AddTask(task);
        }

        [Route("UpdateTask")]
        [HttpPost]
        public void UpdateTask(TaskModel task)
        {
            _taskData.UpdateTask(task);
        }

        [Route("GetAllTaskPriorities")]
        [HttpGet]
        public IEnumerable<TaskPriorityModel> GetAllTaskPriorities()
        {
            return _taskData.GetAllTaskPriorities();
        }

        [Route("GetAllTaskStatuses")]
        [HttpGet]
        public IEnumerable<TaskStatusModel> GetAllTaskStatuses()
        {
            return _taskData.GetAllTaskStatuses();
        }

        [Route("GetAllTaskTypes")]
        [HttpGet]
        public IEnumerable<TaskTypeModel> GetAllTaskTypes()
        {
            return _taskData.GetAllTaskTypes();
        }

        [Authorize(Roles = "Admin")]
        [Route("AddTaskStatus")]
        [HttpPost]
        public void AddTaskStatus(TaskStatusModel taskStatus)
        {
            _taskData.AddTaskStatus(taskStatus);
        }

        [Authorize(Roles = "Admin")]
        [Route("AddTaskType")]
        [HttpPost]
        public void AddTaskType(TaskTypeModel taskType)
        {
            _taskData.AddTaskType(taskType);
        }

        [Authorize(Roles = "Admin")]
        [Route("AddTaskPriority")]
        [HttpPost]
        public void AddTaskPriority(TaskPriorityModel taskPriority)
        {
            _taskData.AddTaskPriority(taskPriority);
        }

        [Route("GetStatusHistory")]
        [HttpGet]
        public IEnumerable<TaskStatusHistoryModel> GetStatusHistory(int taskId)
        {
            return _taskData.GetStatusHistory(taskId);
        }
    }
}
