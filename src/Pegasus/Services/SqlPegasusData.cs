using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pegasus.Entities;
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

        public IEnumerable<Project> GetAllProjects()
        {
            return _context.Projects;
        }

        public IEnumerable<ProjectTask> GetAllTasks()
        {
            return _context.ProjectTasks;
        }

        public IEnumerable<TaskComment> GetAllComments()
        {
            return _context.TaskComments;
        }

        public Project GetProject(int id)
        {
            return _context.Projects.FirstOrDefault(p => p.Id == id);
        }

        public Project AddProject(Project project)
        {
            var entityEntry = _context.Projects.Add(project);
            _context.SaveChanges();
            return entityEntry.Entity;
        }

        public ProjectTask GetTask(int id)
        {
            return _context.ProjectTasks.FirstOrDefault(t => t.Id == id);
        }

        public IEnumerable<ProjectTask> GetTasks(int projectId)
        {
            return _context.ProjectTasks.Where(t => t.ProjectId == projectId);
        }

        public void AddTask(ProjectTask projectTask)
        {
            _context.ProjectTasks.Add(projectTask);
            _context.SaveChanges();

            AddStatusHistory(projectTask);
        }

        public void EditTask(ProjectTask projectTask)
        {
            _context.ProjectTasks.Update(projectTask);
            _context.SaveChanges();
        }

        public TaskComment GetComment(int id)
        {
            return _context.TaskComments.FirstOrDefault(c => c.Id == id);
        }

        public IEnumerable<TaskComment> GetComments(int taskId)
        {
            return _context.TaskComments.Where(c => c.TaskId == taskId);
        }

        public TaskComment AddComment(TaskComment taskComment)
        {
            var entityEntry =  _context.TaskComments.Add(taskComment);
            _context.SaveChanges();
            return entityEntry.Entity;
        }

        public IEnumerable<TaskStatus> GetAllTaskStatuses()
        {
            return _context.TaskStatus;
        }

        public TaskStatus AddTaskStatus(TaskStatus taskStatus)
        {
            var entityEntry = _context.Add(taskStatus);
            _context.SaveChanges();
            return entityEntry.Entity;
        }

        public IEnumerable<TaskType> GetAllTaskTypes()
        {
            return _context.TaskTypes;
        }

        public TaskType AddTaskType(TaskType taskType)
        {
            var entityEntry = _context.Add(taskType);
            _context.SaveChanges();
            return entityEntry.Entity;
        }

        public TaskStatusHistory AddStatusHistory(TaskStatusHistory taskStatusHistory)
        {
            var entityEntry = _context.StatusHistory.Add(taskStatusHistory);
            _context.SaveChanges();
            return entityEntry.Entity;
        }

        public TaskStatusHistory GetTaskCurrentStatusHistory(int taskId)
        {
            return _context.StatusHistory.Where(h => h.TaskId == taskId)
                .OrderByDescending(h => h.Created)
                .FirstOrDefault();
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
    }
}