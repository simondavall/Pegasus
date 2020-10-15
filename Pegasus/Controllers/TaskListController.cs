using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Pegasus.Domain;
using Pegasus.Entities;
using Pegasus.Extensions;
using Pegasus.Models;
using Pegasus.Models.TaskList;
using Pegasus.Services;

namespace Pegasus.Controllers
{
    public class TaskListController : Controller
    {
        private readonly IPegasusData _db;
        private readonly Settings _settings;
        private readonly Cookies _cookies;

        public TaskListController(IPegasusData pegasusData, IConfiguration configuration)
        {
            _db = pegasusData;
            _settings = new Settings(configuration);
            _cookies = new Cookies(configuration);
        }

        public IActionResult Index()
        {
            var projectId = _settings.GetSetting(Request, "Project.Id");

            var taskFilterId = _settings.GetSetting(Request, "taskFilterId");
            _cookies.WriteCookie(Response, "taskFilterId", taskFilterId.ToString());

            Project project = _db.GetProject(projectId) ?? new Project { Id = 0, Name = "All" };
            _cookies.WriteCookie(Response,"Project.Id", projectId.ToString());

            var projectTasks = ProjectTaskExt.Convert(projectId > 0 ? _db.GetTasks(projectId) : _db.GetAllTasks());

            IndexViewModel model =
                new IndexViewModel(projectTasks)
                {
                    ProjectId = projectId,
                    TaskFilterId = taskFilterId,
                    Projects = _db.GetAllProjects(),
                    Project = project
                };

            if (Request != null && Request.IsAjaxRequest())
            {
                return PartialView("_ProjectTaskList", model);
            }

            return View("../TaskList/Index", model);
        }

        public async Task<IActionResult> Create()
        {
            var projectId = _settings.GetSetting(Request, "Project.Id");
            var project = _db.GetProject(projectId);
            var projectTask = new ProjectTask
            {
                ProjectId = projectId,
                TaskRef = await _db.GetNextTaskRef(projectId, project.ProjectPrefix)
            };
            var model = TaskViewModel.Create(new TaskViewModelArgs { PegasusData = _db, ProjectTask = projectTask, Project = project });

            model.ProjectTask = projectTask;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Description,Name,ProjectId,TaskRef,TaskStatusId,TaskTypeId,TaskPriorityId,FixedInRelease")] ProjectTask projectTask)
        {
            projectTask.Created = projectTask.Modified = DateTime.Now;
            if (ModelState.IsValid)
            {
                _db.AddTask(projectTask);
                return RedirectToAction("Index");
            }
            var model = TaskViewModel.Create(new TaskViewModelArgs { PegasusData = _db, ProjectTask = projectTask });

            return View(model);
        }

        public IActionResult Edit(int id)
        {
            var projectTask = _db.GetTask(id);

            _cookies.WriteCookie(Response,"Project.Id", projectTask.ProjectId.ToString());
            var model = TaskViewModel.Create(new TaskViewModelArgs { PegasusData = _db, ProjectTask = projectTask });

            if (Request != null && Request.IsAjaxRequest())
            {
                return PartialView("_EditTaskContent", model);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("Id,Description,Name,Created,ProjectId,TaskRef,TaskStatusId,TaskTypeId,TaskPriorityId,FixedInRelease")] ProjectTask projectTask,
            int existingTaskStatus, string newComment, [Bind("Id,Comment")] IEnumerable<TaskComment> comments)
        {
            if (ModelState.IsValid)
            {
                projectTask.Modified = DateTime.Now;
                _db.UpdateTask(projectTask, existingTaskStatus);
                _db.UpdateComments(comments);
                if (!string.IsNullOrWhiteSpace(newComment))
                {
                    _db.AddComment(
                        new TaskComment
                        {
                            Comment = newComment,
                            Created = DateTime.Now,
                            TaskId = projectTask.Id
                        });
                }
                if (projectTask.IsClosed && projectTask.TaskStatusId != existingTaskStatus)
                {
                    return RedirectToAction("Index");
                }

                return RedirectToAction("Edit", projectTask.Id);
            }

            var taskViewModelArgs =
                new TaskViewModelArgs
                {
                    PegasusData = _db,
                    ProjectTask = projectTask,
                    ExistingStatusId = existingTaskStatus,
                    Comments = comments,
                    NewComment = newComment
                };
            var model = TaskViewModel.Create(taskViewModelArgs);

            return View(model);
        }

        public IActionResult Error()
        {
            var model = new BaseViewModel { ProjectId = 0 };
            return View(model);
        }
    }
}
