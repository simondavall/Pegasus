using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pegasus.Entities.Attributes;
using Pegasus.Library.Api;
using Pegasus.Library.Models;

namespace Pegasus.Controllers
{
    [Authorize(Roles = "Admin")]
    [Require2Fa]
    public class ProjectsController : Controller
    {
        private readonly IProjectsEndpoint _projectsEndpoint;

        public ProjectsController(IProjectsEndpoint projectsEndpoint)
        {
            _projectsEndpoint = projectsEndpoint;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ProjectPrefix")] ProjectModel project)
        {
            if (ModelState.IsValid)
            {
                await _projectsEndpoint.AddProject(project);
                return RedirectToAction("Index");
            }

            return View(project);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var project = await _projectsEndpoint.GetProject((int) id);

            if (project == null) return NotFound();

            return View(project);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _projectsEndpoint.DeleteProject(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var project = await _projectsEndpoint.GetProject((int) id);
            if (project == null) return NotFound();
            return View(project);
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ProjectPrefix")] ProjectModel project)
        {
            if (id != project.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _projectsEndpoint.UpdateProject(project);
                }
                catch (Exception)
                {
                    if (!await ProjectExists(project.Id))
                        return NotFound();
                    throw;
                }

                return RedirectToAction("Index");
            }

            return View(project);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var projects = await _projectsEndpoint.GetAllProjects();
            return View(projects);
        }

        private async Task<bool> ProjectExists(int id)
        {
            var projectFound = await _projectsEndpoint.GetProject(id);
            return projectFound?.Id > 0;
        }
    }
}