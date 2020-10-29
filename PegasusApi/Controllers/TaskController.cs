using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PegasusApi.Library.DataAccess;
using PegasusApi.Library.Models;
using PegasusApi.Models;

namespace PegasusApi.Controllers
{
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




        #region Not yet implemented

        [Obsolete]
        [Route("GetNextTaskRef")]
        [HttpGet]
        public async Task<string> GetNextTaskRef(int projectId, string projectPrefix)
        {
            await Task.Delay(1);
            return string.Empty;

            //old code
            //    var taskIndexer = _context.TaskIndexers.FirstOrDefault(ti => ti.ProjectId == projectId);
            //    if (taskIndexer == null || string.IsNullOrWhiteSpace(projectPrefix))
            //    {
            //        return "###-##";
            //    }
            //    int nextIndex = taskIndexer.NextIndex++;
            //    _context.TaskIndexers.Update(taskIndexer);
            //    await _context.SaveChangesAsync();

            //    return string.Format("{0}-{1}", projectPrefix, nextIndex);
        }

        [Obsolete]
        [Route("AddTaskIndexer")]
        [HttpPost]
        public void AddTaskIndexer(ProjectTaskIndexerModel projectTaskIndexer)
        {


            //old code
            //    _context.TaskIndexers.Add(projectTaskIndexer);
            //    _context.SaveChanges();
        }

        [Obsolete]
        [Route("AddTaskIndexerAsync")]
        [HttpPost]
        public async Task AddTaskIndexerAsync(ProjectTaskIndexerModel projectTaskIndexer)
        {
            await Task.Delay(1);

            //old code
            //    _context.TaskIndexers.Add(projectTaskIndexer);
            //    await _context.SaveChangesAsync();
        }

        internal static async Task DeleteTasksAsync(int projectId)
        {
            // this is a private member used internally only
            await Task.Delay(1);

            //old code
            //    //todo turn this into an async call with ToListAsync() i.e. change return type to IQueryable
            //    var tasksToDelete = GetTasks(projectId).ToList();
            //    foreach (var task in tasksToDelete)
            //    {
            //        _context.TaskComments.RemoveRange(GetComments(task.Id));
            //        _context.StatusHistory.RemoveRange(await GetStatusHistory(task.Id).ToListAsync());
            //    }
            //    _context.ProjectTasks.RemoveRange(tasksToDelete);
            //    await _context.SaveChangesAsync();;
        }

        [Obsolete]
        [Route("AddTaskStatus")]
        [HttpPost]
        public void AddTaskStatus(TaskStatusModel taskStatus)
        {

            //old code
            //    _context.Add(taskStatus);
            //    _context.SaveChanges();
        }

        [Obsolete]
        [Route("AddTaskType")]
        [HttpPost]
        public void AddTaskType(TaskTypeModel taskType)
        {

            //old code
            //    _context.Add(taskType);
            //    _context.SaveChanges();
        }

        [Obsolete]
        [Route("AddTaskPriority")]
        [HttpPost]
        public void AddTaskPriority(TaskPriorityModel taskPriority)
        {

            //old code
            //    _context.Add(taskPriority);
            //    _context.SaveChanges();
        }

        //private void AddStatusHistory(ProjectTaskModel task)
        //{

        //    //old code
        //    //    var taskStatusHistory = new TaskStatusHistory
        //    //    {
        //    //        TaskId = task.Id,
        //    //        TaskStatusId = task.TaskStatusId,
        //    //        Created = task.Modified
        //    //    };
        //    //    _context.StatusHistory.Add(taskStatusHistory);
        //    //    _context.SaveChanges();
        //}

        [Obsolete]
        [Route("GetStatusHistory")]
        [HttpGet]
        public IEnumerable<TaskStatusHistoryModel> GetStatusHistory(int taskId)
        {
            return new[] { new TaskStatusHistoryModel(), new TaskStatusHistoryModel() };

            //old code
            //    return _context.StatusHistory.Where(sh => sh.TaskId == taskId);
        }

        #endregion


    }
}
