using System;
using System.Linq;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ProjectPrefix")] ProjectModel project)
        {
            if (!ModelState.IsValid)
            {
                return View(project);
            }

            await _projectsEndpoint.AddProject(project);
            return RedirectToAction(nameof(Index));
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
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var project = await _projectsEndpoint.GetProject((int) id);
            if (project == null) return NotFound();
            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ProjectPrefix,IsPinned,IsActive")] ProjectModel project)
        {
            if (id != project.Id) return NotFound();

            if (!ModelState.IsValid) return View(project);

            try
            {
                await _projectsEndpoint.UpdateProject(project);
            }
            catch (Exception ex)
            {
                if (!await ProjectExists(project.Id))
                    return NotFound();
                //TODO Need to log this error
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(project);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var projects = await _projectsEndpoint.GetAllProjects();
            return View(projects.OrderBy(x => x.Name));
        }

        private async Task<bool> ProjectExists(int id)
        {
            var projectFound = await _projectsEndpoint.GetProject(id);
            return projectFound?.Id > 0;
        }
    }
}