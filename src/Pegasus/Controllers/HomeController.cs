using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            var projectId = GetSettingId(Request, "Project.Id");
            WriteCookie("Project.Id", projectId.ToString());

            var taskFilterId = GetSettingId(Request, "taskFilterId");
            WriteCookie("taskFilterId", taskFilterId.ToString());

            IndexViewModel model = new IndexViewModel
            {
                ProjectId = projectId,
                TaskFilterId = taskFilterId,
                ProjectTasks = ProjectTaskExt.Convert(projectId > 0 ? _db.GetTasks(projectId) : _db.GetAllTasks()),
                Projects = _db.GetAllProjects(),
                Project = projectId > 0 ? _db.GetProject(projectId) : new Project { Id = 0, Name = "All" }
            };

            Func<ProjectTaskExt, bool> whereClause = SetWhereClause(taskFilterId);
            model.ProjectTasks = model.ProjectTasks.Where(whereClause);

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
            var model = TaskViewModel.Create(_db, projectTask, project);

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
            var model = TaskViewModel.Create(_db, projectTask, _db.GetProject(projectTask.ProjectId));
            model.ButtonText = model.Action = "Create";

            return View("Edit", model);
        }

        public IActionResult Edit(int id)
        {
            var projectTask = _db.GetTask(id);
            var project = _db.GetProject(projectTask.ProjectId);

            var model = TaskViewModel.Create(_db, projectTask, project);
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
            var model = TaskViewModel.Create(_db, projectTask, project, existingTaskStatus);
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

        private Func<ProjectTaskExt, bool> SetWhereClause(int taskFilterId)
        {
            switch (taskFilterId)
            {
                case 1:
                    return pt => !pt.IsClosed;
                case 2:
                    return pt => pt.TaskPriorityId > 3;
                default:
                    return pt => true; 
            }
        }

        private void WriteCookie(string setting, string settingVaue)
        {
            //todo get expiry from config
            CookieOptions options = new CookieOptions {Expires = new DateTimeOffset(DateTime.Now.AddDays(30))};
            Response.Cookies.Append(setting, settingVaue, options);
        }

    }
}
