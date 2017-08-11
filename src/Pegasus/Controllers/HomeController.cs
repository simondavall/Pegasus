using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pegasus.Entities;
using Pegasus.Services;
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
            IndexViewModel model = new IndexViewModel
            {
                ProjectTasks = _db.GetAllTasks(),
                Projects = _db.GetAllProjects()
            };

            return View(model);
        }

        public IActionResult Details()
        {
            return View();
        }

        public async Task<IActionResult> Create()
        {
            CreateViewModel model = new CreateViewModel();
            model.TaskTypes = new SelectList(_db.GetAllTaskTypes(), "Id", "Name", 1);
            model.TaskStatuses = new SelectList(_db.GetAllTaskStatuses(), "Id", "Name", 1);
            // todo activate the following with a real project id
            model.Project = _db.GetProject(1);

            var projectTask = new ProjectTask();
            projectTask.ProjectId = model.Project.Id;
            projectTask.TaskRef = await _db.GetNextTaskRef(model.Project.Id, model.Project.ProjectPrefix);
            model.ProjectTask = projectTask;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Description,Name,ProjectId,TaskRef,TaskStatusId,TaskTypeId")] ProjectTask projectTask)
        {
            projectTask.Created = projectTask.Modified = DateTime.Now;
            if (ModelState.IsValid)
            {
                _db.AddTask(projectTask);
                return RedirectToAction("Index");
            }
            CreateViewModel model = new CreateViewModel();
            model.TaskTypes = new SelectList(_db.GetAllTaskTypes(), "Id", "Name", 1);
            model.TaskStatuses = new SelectList(_db.GetAllTaskStatuses(), "Id", "Name", 1);
            model.ProjectTask = projectTask;
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            var projectTask = _db.GetTask(id);

            CreateViewModel model = new CreateViewModel();
            model.TaskTypes = new SelectList(_db.GetAllTaskTypes(), "Id", "Name", 1);
            model.TaskStatuses = new SelectList(_db.GetAllTaskStatuses(), "Id", "Name", 1);
            model.Project = _db.GetProject(projectTask.ProjectId);
            model.ProjectTask = projectTask;
            model.ExistingTaskStatus = projectTask.TaskStatusId;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("Id,Description,Name,ProjectId,TaskRef,TaskStatusId,TaskTypeId")] ProjectTask projectTask, int existingTaskStatus)
        {
            projectTask.Modified = DateTime.Now;
            if (ModelState.IsValid)
            {
                _db.UpdateTask(projectTask, existingTaskStatus);
            }

            return Edit(projectTask.Id);
        }
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
