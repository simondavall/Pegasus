using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pegasus.Entities;
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

        public IActionResult Index(int id)
        {
            IndexViewModel model = new IndexViewModel
            {
                ProjectId = id,
                ProjectTasks = ProjectTaskExt.Convert(id > 0 ? _db.GetTasks(id) : _db.GetAllTasks()),
                Projects = _db.GetAllProjects(),
                Project = id > 0 ? _db.GetProject(id) : new Project { Id = 0, Name = "All" }
            };

            return View("Index", model);
        }

        public IActionResult Details()
        {
            return View();
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
                return RedirectToAction("Index", new { id = projectTask.ProjectId });
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
                return RedirectToAction("Index", new {id = projectTask.ProjectId});
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

    }
}
