using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PegasusApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PegasusApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        [Route("/GetAllProjects")]
        [HttpGet]
        public IEnumerable<ProjectModel> GetAllProjects()
        {
            return new[] { new ProjectModel(), new ProjectModel() };

            //old code
            //return _context.Projects;
        }

        [Route("/UpdateProject")]
        [HttpPost]
        public void UpdateProject([FromBody] ProjectModel project)
        {
            // update project

            //old code
            //    _context.Projects.Update(project);
            //    _context.SaveChanges();
        }

        [Route("/UpdateProjectAsync")]
        [HttpPost]
        public async Task UpdateProjectAsync([FromBody] ProjectModel project)
        {
            await Task.Delay(1);
            // update project async

            // old code (I think api controller already works asynchronously)
            //    _context.Projects.Update(project);
            //    await _context.SaveChangesAsync();
        }

        [Route("/DeleteProjectAsync")]
        [HttpDelete]
        public void DeleteProjectAsync(ProjectModel project)
        {

            // old code
            //    _context.TaskIndexers.RemoveRange(_context.TaskIndexers.Where(ti => ti.ProjectId == project.Id));
            //    await DeleteTasksAsync(project.Id);
            //    _context.Projects.Remove(project);
            //    await _context.SaveChangesAsync();
        }

        [Route("/GetProject")]
        [HttpGet]
        public ProjectModel GetProject(int id)
        {
            return new ProjectModel();

            //old code
            //    return _context.Projects.FirstOrDefault(p => p.Id == id);
        }

        [Route("/GetProjectAsync")]
        [HttpGet]
        public async Task<ProjectModel> GetProjectAsync(int id)
        {
            //return await new Project();
            await Task.Delay(1);
            return null;

            //old code
            //    return await _context.Projects.SingleOrDefaultAsync(p => p.Id == id);
        }
        
        [Route("/SaveProject")]
        [HttpPost]
        public void SaveProject(ProjectModel project)
        {
            // ProjectData data = new ProjectData();
            // data.SaveProjectRecord(project)

            //old code
            //    _context.Projects.Add(project);
            //    _context.SaveChanges();
            //    AddTaskIndexer(new ProjectTaskIndexer { NextIndex = 1, ProjectId = project.Id });
        }

        [Route("/SaveProjectAsync")]
        [HttpPost]
        public async Task SaveProjectAsync(ProjectModel project)
        {
            await Task.Delay(1);
            // ProjectData data = new ProjectData();
            // data.SaveProjectRecord(project)

            //old code
            //    _context.Projects.Add(project);
            //    await _context.SaveChangesAsync();
            //    await AddTaskIndexerAsync(new ProjectTaskIndexer { NextIndex = 1, ProjectId = project.Id });
        }
    }
}
