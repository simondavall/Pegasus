using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PegasusApi.Models;

namespace PegasusApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        [Route("/GetAllTasks")]
        [HttpGet]
        public IEnumerable<ProjectTaskModel> GetAllTasks()
        {
            return new[] { new ProjectTaskModel(), new ProjectTaskModel() };

            //old code
            //return _context.ProjectTasks.OrderByDescending(t => t.Created);
        }

        [Route("/GetTask")]
        [HttpGet]
        public ProjectTaskModel GetTask(int id)
        {
            return new ProjectTaskModel();

            //old code
            //    return _context.ProjectTasks.FirstOrDefault(t => t.Id == id);
        }

        [Route("/GetTasks")]
        [HttpGet]
        public IEnumerable<ProjectTaskModel> GetTasks(int projectId)
        {
            return new[] { new ProjectTaskModel(), new ProjectTaskModel() };

            //old code
            //    return _context.ProjectTasks.Where(t => t.ProjectId == projectId)
            //        .OrderByDescending(t => t.Created);
        }

        [Route("/AddTask")]
        [HttpPost]
        public void AddTask(ProjectTaskModel projectTask)
        {
            

            //old code
            //    _context.ProjectTasks.Add(projectTask);
            //    _context.SaveChanges();

            //    AddStatusHistory(projectTask);
        }

        [Route("/UpdateTask")]
        [HttpPost]
        public void UpdateTask(ProjectTaskModel projectTask, int existingTaskStatus)
        {


            //old code
            //    _context.ProjectTasks.Update(projectTask);
            //    _context.SaveChanges();

            //    if (projectTask.TaskStatusId != existingTaskStatus)
            //    {
            //        AddStatusHistory(projectTask);
            //    }
        }

        [Route("/GetNextTaskRef")]
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

        [Route("/AddTaskIndexer")]
        [HttpPost]
        public void AddTaskIndexer(ProjectTaskIndexerModel projectTaskIndexer)
        {


            //old code
            //    _context.TaskIndexers.Add(projectTaskIndexer);
            //    _context.SaveChanges();
        }

        [Route("/AddTaskIndexerAsync")]
        [HttpPost]
        public async Task AddTaskIndexerAsync(ProjectTaskIndexerModel projectTaskIndexer)
        {
            await Task.Delay(1);

            //old code
            //    _context.TaskIndexers.Add(projectTaskIndexer);
            //    await _context.SaveChangesAsync();
        }

        private async Task DeleteTasksAsync(int projectId)
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

        [Route("/GetAllTaskStatuses")]
        [HttpGet]
        public IEnumerable<TaskStatusModel> GetAllTaskStatuses()
        {
            return new[] { new TaskStatusModel(), new TaskStatusModel() };

            //old code
            //    return _context.TaskStatus.OrderBy(s => s.DisplayOrder);
        }

        [Route("/AddTaskStatus")]
        [HttpPost]
        public void AddTaskStatus(TaskStatusModel taskStatus)
        {

            //old code
            //    _context.Add(taskStatus);
            //    _context.SaveChanges();
        }

        [Route("/GetAllTaskTypes")]
        [HttpGet]
        public IEnumerable<TaskTypeModel> GetAllTaskTypes()
        {
            return new[] { new TaskTypeModel(), new TaskTypeModel() };

            //old code
            //    return _context.TaskTypes.OrderBy(t => t.DisplayOrder);
        }

        [Route("/AddTaskType")]
        [HttpPost]
        public void AddTaskType(TaskTypeModel taskType)
        {

            //old code
            //    _context.Add(taskType);
            //    _context.SaveChanges();
        }

        [Route("/GetAllTaskPriorities")]
        [HttpGet]
        public IEnumerable<TaskPriorityModel> GetAllTaskPriorities()
        {
            return new[] { new TaskPriorityModel(), new TaskPriorityModel() };

            //old code
            //    return _context.TaskPriorities.OrderBy(tp => tp.DisplayOrder);
        }

        [Route("/AddTaskPriority")]
        [HttpPost]
        public void AddTaskPriority(TaskPriorityModel taskPriority)
        {

            //old code
            //    _context.Add(taskPriority);
            //    _context.SaveChanges();
        }

        private void AddStatusHistory(ProjectTaskModel task)
        {

            //old code
            //    var taskStatusHistory = new TaskStatusHistory
            //    {
            //        TaskId = task.Id,
            //        TaskStatusId = task.TaskStatusId,
            //        Created = task.Modified
            //    };
            //    _context.StatusHistory.Add(taskStatusHistory);
            //    _context.SaveChanges();
        }

        [Route("/GetStatusHistory")]
        [HttpGet]
        public IEnumerable<TaskStatusHistoryModel> GetStatusHistory(int taskId)
        {
            return new[] { new TaskStatusHistoryModel(), new TaskStatusHistoryModel() };

            //old code
            //    return _context.StatusHistory.Where(sh => sh.TaskId == taskId);
        }
    }
}
