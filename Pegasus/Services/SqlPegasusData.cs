using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pegasus.Entities;
using Project = Pegasus.Entities.Project;
using TaskStatus = Pegasus.Entities.TaskStatus;

namespace Pegasus.Services
{
    public class SqlPegasusData : IPegasusData
    {
        private readonly PegasusDbContext _context;

        public SqlPegasusData(PegasusDbContext context)
        {
            _context = context;
        }

 // Project related queries
   
        public IQueryable<Project> GetAllProjects()
        {
            return _context.Projects;
        }

        public void UpdateProject(Project project)
        {
            _context.Projects.Update(project);
            _context.SaveChanges();
        }
        public async Task UpdateProjectAsync(Project project)
        {
            _context.Projects.Update(project);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProjectAsync(Project project)
        {
            _context.TaskIndexers.RemoveRange(_context.TaskIndexers.Where(ti => ti.ProjectId == project.Id));
            await DeleteTasksAsync(project.Id);
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
        }

        public Project GetProject(int id)
        {
            return _context.Projects.FirstOrDefault(p => p.Id == id);
        }
        public async Task<Project> GetProjectAsync(int? id)
        {
            return await _context.Projects.SingleOrDefaultAsync(p => p.Id == id);
        }

        public void AddProject(Project project)
        {
            _context.Projects.Add(project);
            _context.SaveChanges();
            AddTaskIndexer(new ProjectTaskIndexer {NextIndex = 1, ProjectId = project.Id});
        }
        public async Task AddProjectAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            await AddTaskIndexerAsync(new ProjectTaskIndexer { NextIndex = 1, ProjectId = project.Id });
        }


// Task related queries

        public IEnumerable<ProjectTask> GetAllTasks()
        {
            return _context.ProjectTasks.OrderByDescending(t => t.Created);
        }

        public ProjectTask GetTask(int id)
        {
            return _context.ProjectTasks.FirstOrDefault(t => t.Id == id);
        }

        public IEnumerable<ProjectTask> GetTasks(int projectId)
        {
            return _context.ProjectTasks.Where(t => t.ProjectId == projectId)
                .OrderByDescending(t => t.Created);
        }

        public void AddTask(ProjectTask projectTask)
        {
            _context.ProjectTasks.Add(projectTask);
            _context.SaveChanges();

            AddStatusHistory(projectTask);
        }

        public void UpdateTask(ProjectTask projectTask, int existingTaskStatus)
        {
            _context.ProjectTasks.Update(projectTask);
            _context.SaveChanges();

            if (projectTask.TaskStatusId != existingTaskStatus)
            {
                AddStatusHistory(projectTask);
            }
        }

        public async Task<string> GetNextTaskRef(int projectId, string projectPrefix)
        {
            var taskIndexer = _context.TaskIndexers.FirstOrDefault(ti => ti.ProjectId == projectId);
            if (taskIndexer == null || string.IsNullOrWhiteSpace(projectPrefix))
            {
                return "###-##";
            }
            int nextIndex = taskIndexer.NextIndex++;
            _context.TaskIndexers.Update(taskIndexer);
            await _context.SaveChangesAsync();

            return string.Format("{0}-{1}", projectPrefix, nextIndex);
        }

        public void AddTaskIndexer(ProjectTaskIndexer projectTaskIndexer)
        {
            _context.TaskIndexers.Add(projectTaskIndexer);
            _context.SaveChanges();
        }
        public async Task AddTaskIndexerAsync(ProjectTaskIndexer projectTaskIndexer)
        {
            _context.TaskIndexers.Add(projectTaskIndexer);
            await _context.SaveChangesAsync();
        }

        private async Task DeleteTasksAsync(int projectId)
        {
            //todo turn this into an async call with ToListAsync() i.e. change return type to IQueryable
            var tasksToDelete = GetTasks(projectId).ToList();
            foreach (var task in tasksToDelete)
            {
                _context.TaskComments.RemoveRange(GetComments(task.Id));
                _context.StatusHistory.RemoveRange(await GetStatusHistory(task.Id).ToListAsync());
            }
            _context.ProjectTasks.RemoveRange(tasksToDelete);
            await _context.SaveChangesAsync();
        }

        // Comment related queries

        public TaskComment GetComment(int id)
        {
            return _context.TaskComments.FirstOrDefault(c => c.Id == id);
        }

        public IEnumerable<TaskComment> GetComments(int taskId)
        {
            return _context.TaskComments.Where(c => c.TaskId == taskId && !c.IsDeleted).OrderBy(c => c.Created);
        }

        public void AddComment(TaskComment taskComment)
        {
            _context.TaskComments.Add(taskComment);
            _context.SaveChanges();
        }

        public void UpdateComment(TaskComment taskComment)
        {
            _context.TaskComments.Update(taskComment);
            _context.SaveChanges();
        }

        public void UpdateComments(IEnumerable<TaskComment> taskComments)
        {
            foreach (var taskComment in taskComments)
            {
                _context.TaskComments.Update(taskComment);
            }
            _context.SaveChanges();
        }

        // All other queries

        public IEnumerable<TaskStatus> GetAllTaskStatuses()
        {
            return _context.TaskStatus.OrderBy(s => s.DisplayOrder);
        }

        public void AddTaskStatus(TaskStatus taskStatus)
        {
            _context.Add(taskStatus);
            _context.SaveChanges();
        }

        public IEnumerable<TaskType> GetAllTaskTypes()
        {
            return _context.TaskTypes.OrderBy(t => t.DisplayOrder);
        }

        public void AddTaskType(TaskType taskType)
        {
            _context.Add(taskType);
            _context.SaveChanges();
        }

        public IEnumerable<TaskPriority> GetAllTaskPriorities()
        {
            return _context.TaskPriorities.OrderBy(tp => tp.DisplayOrder);
        }

        public void AddTaskPriority(TaskPriority taskPriority)
        {
            _context.Add(taskPriority);
            _context.SaveChanges();
        }

        private void AddStatusHistory(ProjectTask task)
        {
            var taskStatusHistory = new TaskStatusHistory
            {
                TaskId = task.Id,
                TaskStatusId = task.TaskStatusId,
                Created = task.Modified
            };
            _context.StatusHistory.Add(taskStatusHistory);
            _context.SaveChanges();
        }

        public IQueryable<TaskStatusHistory> GetStatusHistory(int taskId)
        {
            return _context.StatusHistory.Where(sh => sh.TaskId == taskId);
        }
    }
}