using System.Collections.Generic;
using System.Threading.Tasks;
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

        [Route("AddTask")]
        [HttpPost]
        public async Task AddTask(TaskModel task)
        {
            await _taskData.AddTask(task);
        }
        
        [Authorize(Roles = "Admin")]
        [Route("AddTaskStatus")]
        [HttpPost]
        public async Task AddTaskStatus(TaskStatusModel taskStatus)
        {
            await _taskData.AddTaskStatus(taskStatus);
        }

        [Authorize(Roles = "Admin")]
        [Route("AddTaskType")]
        [HttpPost]
        public async Task AddTaskType(TaskTypeModel taskType)
        {
            await _taskData.AddTaskType(taskType);
        }

        [Authorize(Roles = "Admin")]
        [Route("AddTaskPriority")]
        [HttpPost]
        public async Task AddTaskPriority(TaskPriorityModel taskPriority)
        {
            await _taskData.AddTaskPriority(taskPriority);
        }

        [Route("GetAllTasks")]
        [HttpGet]
        public async Task<IEnumerable<TaskModel>> GetAllTasks()
        {
            return await _taskData.GetAllTasks();
        }

        [Route("GetAllTaskPriorities")]
        [HttpGet]
        public async Task<IEnumerable<TaskPriorityModel>> GetAllTaskPriorities()
        {
            return await _taskData.GetAllTaskPriorities();
        }

        [Route("GetAllTaskStatuses")]
        [HttpGet]
        public async Task<IEnumerable<TaskStatusModel>> GetAllTaskStatuses()
        {
            return await _taskData.GetAllTaskStatuses();
        }

        [Route("GetAllTaskTypes")]
        [HttpGet]
        public async Task<IEnumerable<TaskTypeModel>> GetAllTaskTypes()
        {
            return await _taskData.GetAllTaskTypes();
        }

        [Route("GetStatusHistory")]
        [HttpGet]
        public async Task<IEnumerable<TaskStatusHistoryModel>> GetStatusHistory(int taskId)
        {
            return await _taskData.GetStatusHistory(taskId);
        }

        [Route("GetTask/{taskId}")]
        [HttpGet]
        public async Task<TaskModel> GetTask(int taskId)
        {
            return await _taskData.GetTask(taskId);
        }

        [Route("GetTasks/{projectId}")]
        [HttpGet]
        public async Task<IEnumerable<TaskModel>> GetTasks(int projectId)
        {
            return await _taskData.GetTasks(projectId);
        }

        [Route("UpdateTask")]
        [HttpPost]
        public async Task UpdateTask(TaskModel task)
        {
            await _taskData.UpdateTask(task);
        }
    }
}
