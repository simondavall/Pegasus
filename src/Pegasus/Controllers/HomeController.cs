using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pegasus.Entities;
using Pegasus.Extensions;
using Pegasus.Services;
using Pegasus.ViewModels;
using Pegasus.ViewModels.Home;

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
            var projectId = GetProjectId(Request);
            WriteCookie("Project.Id", projectId.ToString());

            IndexViewModel model = new IndexViewModel
            {
                ProjectId = projectId,
                ProjectTasks = ProjectTaskExt.Convert(projectId > 0 ? _db.GetTasks(projectId) : _db.GetAllTasks()),
                Projects = _db.GetAllProjects(),
                Project = projectId > 0 ? _db.GetProject(projectId) : new Project { Id = 0, Name = "All" }
            };

            if (Request != null && Request.IsAjaxRequest())
            {
                return PartialView("_ProjectTaskList", model);
            }

            return View("Index", model);
        }

        public async Task<IActionResult> Create(int id)
        {
            var project = _db.GetProject(id);
            var projectTask = new ProjectTask
            {
                ProjectId = id,
                TaskRef = await _db.GetNextTaskRef(id, project.ProjectPrefix)
            };
            var model = CreateTaskViewModel(projectTask, project);

            model.ProjectTask = projectTask;
            model.ButtonText = model.Action = "Create";

            return View("Edit", model);
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
            var model = CreateTaskViewModel(projectTask, _db.GetProject(projectTask.ProjectId));
            model.ButtonText = model.Action = "Create";

            return View("Edit", model);
        }

        public IActionResult Edit(int id)
        {
            var projectTask = _db.GetTask(id);
            var project = _db.GetProject(projectTask.ProjectId);

            TaskViewModel model = CreateTaskViewModel(projectTask, project);
            model.Action = "Edit";
            model.ButtonText = "Update";

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("Id,Description,Name,Created,ProjectId,TaskRef,TaskStatusId,TaskTypeId,TaskPriorityId,FixedInRelease")] ProjectTask projectTask, int existingTaskStatus)
        {
            projectTask.Modified = DateTime.Now;
            if (ModelState.IsValid)
            {
                _db.UpdateTask(projectTask, existingTaskStatus);
                return RedirectToAction("Index");
            }

            var project = _db.GetProject(projectTask.ProjectId);
            TaskViewModel model = CreateTaskViewModel(projectTask, project, existingTaskStatus);
            model.Action = "Edit";
            model.ButtonText = "Update";

            return View(model);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            var model = new BaseViewModel {ProjectId = 0};

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


        private TaskViewModel CreateTaskViewModel(ProjectTask projectTask, Project project)
        {
            return CreateTaskViewModel(projectTask, project, projectTask.TaskStatusId);
        }

        private TaskViewModel CreateTaskViewModel(ProjectTask projectTask, Project project, int existingTaskStatus)
        {
            return
                new TaskViewModel
                {
                    ProjectId = projectTask.ProjectId,
                    TaskTypes = new SelectList(_db.GetAllTaskTypes(), "Id", "Name", 1),
                    TaskStatuses = new SelectList(_db.GetAllTaskStatuses(), "Id", "Name", 1),
                    TaskPriorities = new SelectList(_db.GetAllTaskPriorities(), "Id", "Name", 1),
                    ProjectTask = projectTask,
                    Project = project,
                    ExistingTaskStatus = existingTaskStatus
                };
        }

        private int GetProjectId(HttpRequest request)
        {
            int projectId = 0;

            if (request == null)
                return projectId;

            string fromRequest =  request.Query["Project.Id"];
            if (!string.IsNullOrWhiteSpace(fromRequest) && int.TryParse(fromRequest, out projectId))
                return projectId;

            string fromCookie = request.Cookies["Project.Id"];
            if (!string.IsNullOrWhiteSpace(fromCookie) && int.TryParse(fromCookie, out projectId))
                return projectId;

            return 0;
        }

        private void WriteCookie(string setting, string settingVaue)
        {
            //todo get expiry from config
            CookieOptions options = new CookieOptions {Expires = new DateTimeOffset(DateTime.Now.AddDays(30))};
            Response.Cookies.Append(setting, settingVaue, options);
        }
    }
}
