using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PegasusApi.Library.DataAccess;
using PegasusApi.Library.Models;

namespace PegasusApi.Controllers
{
    [Authorize(Roles = "PegasusUser")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectsData _projectsData;

        public ProjectController(IProjectsData projectsData)
        {
            _projectsData = projectsData;
        }

        [Authorize(Roles = "Admin")]
        [Route("AddProject")]
        [HttpPost]
        public async Task AddProject(ProjectModel project)
        {
            await _projectsData.AddProject(project);
        }

        [Authorize(Roles = "Admin")]
        [Route("DeleteProject/{id}")]
        [HttpDelete]
        public async Task DeleteProject(int id)
        {
            await _projectsData.DeleteProject(id);
        }

        [Route("GetAllProjects")]
        [HttpGet]
        public async Task<List<ProjectModel>> GetAllProjects()
        {
            return await _projectsData.GetProjects();
        }

        [Route("GetProject/{id}")]
        [HttpGet]
        public async Task<ProjectModel> GetProject(int id)
        {
            return await _projectsData.GetProject(id);
        }

        [Authorize(Roles = "Admin")]
        [Route("UpdateProject")]
        [HttpPost]
        public async Task UpdateProject(ProjectModel project)
        {
            await _projectsData.UpdateProject(project);
        }
    }
}