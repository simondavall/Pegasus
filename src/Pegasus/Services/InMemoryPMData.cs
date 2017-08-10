using System;
using System.Collections.Generic;
using System.Linq;
using Pegasus.Entities;

namespace Pegasus.Services
{
    public class InMemoryPegasusData //: IPegasusData
    {
        private List<Project> _projects;
        private List<ProjectTask> _projectTasks;
        private List<TaskComment> _taskComments;
        private List<TaskStatus> _taskStatuses;
        private List<TaskType> _taskTypes;
        private List<TaskStatusHistory> _taskHistory;

        public InMemoryPegasusData()
        {
            _taskStatuses = new List<TaskStatus>
            {
                new TaskStatus {Id = 1, Name = "Submitted", DisplayOrder = 0},
                new TaskStatus {Id = 2, Name = "In Progress", DisplayOrder = 1},
                new TaskStatus {Id = 3, Name = "Completed", DisplayOrder = 3}
            };
            _taskTypes = new List<TaskType>
            {
                new TaskType {Id = 1, Name = "Bug", DisplayOrder = 0},
                new TaskType {Id = 2, Name = "Task", DisplayOrder = 1}
            };
            _projects = new List<Project>
            {
                new Project {Id = 1, Name = "Project1"},
                new Project {Id = 2, Name = "Project2"}
            };
            _projectTasks = new List<ProjectTask>
            {
                new ProjectTask
                {
                    Id = 1,
                    TaskRef = "P1-001",
                    Name = "Add initial project framework",
                    TaskTypeId = 2,
                    ProjectId = 1,
                    Modified = DateTime.Now.AddMinutes(-13),
                    Created = DateTime.Now.AddMinutes(-13)
                },
                new ProjectTask
                {
                    Id = 2,
                    TaskRef = "P1-002",
                    Name = "Connect to sql server db",
                    TaskTypeId = 1,
                    ProjectId = 1,
                    Modified = DateTime.Now.AddMinutes(-9),
                    Created = DateTime.Now.AddMinutes(-9)
                },
                new ProjectTask
                {
                    Id = 3,
                    TaskRef = "P2-001",
                    Name = "Start a new AspNetCore project",
                    TaskTypeId = 2,
                    ProjectId = 2,
                    Modified = DateTime.Now.AddMinutes(-5),
                    Created = DateTime.Now.AddMinutes(-5)
                }
            };
            _taskComments = new List<TaskComment>
            {
                new TaskComment
                {
                    Id = 1,
                    Comment = "Task initiated",
                    Created = DateTime.Now.AddMinutes(-10),
                    TaskId = 1
                }
            };
            _taskHistory = new List<TaskStatusHistory>
            {
                new TaskStatusHistory {Id = 1, TaskId = 1, TaskStatusId = 1, Created = DateTime.Now.AddMinutes(-15)},
                new TaskStatusHistory {Id = 2, TaskId = 2, TaskStatusId = 1, Created = DateTime.Now.AddMinutes(-13)},
                new TaskStatusHistory {Id = 3, TaskId = 3, TaskStatusId = 1, Created = DateTime.Now.AddMinutes(-11)},
                new TaskStatusHistory {Id = 4, TaskId = 2, TaskStatusId = 2, Created = DateTime.Now.AddMinutes(-12)},
            };
        }

        public IEnumerable<Project> GetAllProjects()
        {
            return _projects;
        }

        public IEnumerable<ProjectTask> GetAllTasks()
        {
            return _projectTasks;
        }

        public IEnumerable<TaskComment> GetAllComments()
        {
            return _taskComments;
        }

        public Project GeProject(int id)
        {
            return _projects.FirstOrDefault(p => p.Id == id);
        }

        public void AddProject(Project project)
        {
            _projects.Add(project);
        }

        public ProjectTask GetTask(int id)
        {
            return _projectTasks.FirstOrDefault(t => t.Id == id);
        }

        public IEnumerable<ProjectTask> GetTasks(int projectId)
        {
            return _projectTasks.Where(t => t.ProjectId == projectId);
        }

        public void AddTask(ProjectTask projectTask)
        {
            _projectTasks.Add(projectTask);
        }

        public TaskComment GetComment(int id)
        {
            return _taskComments.FirstOrDefault(c => c.Id == id);
        }

        public IEnumerable<TaskComment> GetComments(int taskId)
        {
            return _taskComments.Where(c => c.TaskId == taskId);
        }

        public void AddComment(TaskComment taskComment)
        {
            _taskComments.Add(taskComment);
        }
    }
}
