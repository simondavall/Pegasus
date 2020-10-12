using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pegasus.Entities;
using Pegasus.Extensions;
using Pegasus.Models;
using Pegasus.Models.Home;
using Pegasus.Services;

namespace Pegasus.Controllers
{
    public class HomeController : Controller
    {
        private IPegasusData _db;

        public HomeController(IPegasusData pegasusData)
        {
            _db = pegasusData;
        }

        public IActionResult Index()
        {
            var projectId = GetSettingId(Request, "Project.Id");

            var taskFilterId = GetSettingId(Request, "taskFilterId");
            WriteCookie("taskFilterId", taskFilterId.ToString());

            Project project = _db.GetProject(projectId) ?? new Project { Id = 0, Name = "All" };
            WriteCookie("Project.Id", projectId.ToString());

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

            return View("Index", model);
        }

        public async Task<IActionResult> Create()
        {
            var projectId = GetSettingId(Request, "Project.Id");
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
            WriteCookie("Project.Id", projectTask.ProjectId.ToString());
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

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            var model = new BaseViewModel { ProjectId = 0 };

            return View(model);
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";
            var model = new BaseViewModel { ProjectId = 0 };

            return View(model);
        }

        public IActionResult Error()
        {
            var model = new BaseViewModel { ProjectId = 0 };
            return View(model);
        }

        private int GetSettingId(HttpRequest request, string settingName, int defaultReturnVal = 0)
        {
            int id = defaultReturnVal;

            if (request == null)
                return id;

            string fromRequest = request.Query[settingName];
            if (!string.IsNullOrWhiteSpace(fromRequest) && int.TryParse(fromRequest, out id))
                return id;

            string fromCookie = request.Cookies[settingName];
            if (!string.IsNullOrWhiteSpace(fromCookie) && int.TryParse(fromCookie, out id))
                return id;

            return defaultReturnVal;
        }


        private void WriteCookie(string setting, string settingVaue)
        {
            //todo get expiry from config
            CookieOptions options = new CookieOptions { Expires = new DateTimeOffset(DateTime.Now.AddDays(30)) };
            Response.Cookies.Append(setting, settingVaue, options);
        }

    }
}
