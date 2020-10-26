using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PegasusApi.Library.DataAccess;
using PegasusApi.Library.Models;

namespace PegasusApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ProjectController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Route("GetAllProjects")]
        [HttpGet]
        public List<ProjectModel> GetAllProjects()
        {
            var data = new ProjectsData(_configuration);
            return data.GetProjects();
        }

        [Route("GetProject/{id}")]
        [HttpGet]
        public ProjectModel GetProject(int id)
        {
            var data = new ProjectsData(_configuration);
            return data.GetProject(id);
        }

        [Route("AddProject")]
        [HttpPost]
        public void AddProject(ProjectModel project)
        {
            var data = new ProjectsData(_configuration);
            data.AddProject(project);
        }

        [Route("UpdateProject")]
        [HttpPost]
        public void UpdateProject(ProjectModel project)
        {
            var data = new ProjectsData(_configuration);
            data.UpdateProject(project);
        }

        [Route("DeleteProject/{id}")]
        [HttpDelete]
        public void DeleteProject(int id)
        {
            var data = new ProjectsData(_configuration);
            data.DeleteProject(id);
        }
    }
}