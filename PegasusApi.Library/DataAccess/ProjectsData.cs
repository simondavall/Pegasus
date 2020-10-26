using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using PegasusApi.Library.Models;

namespace PegasusApi.Library.DataAccess
{
    public class ProjectsData
    {
        private readonly IConfiguration _configuration;
        private const string ConnectionStringName = "Pegasus";

        public ProjectsData(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void AddProject(ProjectModel project)
        {
            var sql = new SqlDataAccess(_configuration);

            var parameters = new { project.Name, project.ProjectPrefix };

            sql.SaveData<dynamic>("spProjects_Add", parameters, ConnectionStringName);
        }

        public void DeleteProject(int id)
        {
            var sql = new SqlDataAccess(_configuration);

            sql.SaveData<dynamic>("spProjects_Delete", new { id }, ConnectionStringName);
        }

        public ProjectModel GetProject(int id)
        {
            var sql = new SqlDataAccess(_configuration);

            var output = sql.LoadData<ProjectModel, dynamic>("spProjects_Get", new { id }, ConnectionStringName);

            return output.FirstOrDefault();
        }

        public List<ProjectModel> GetProjects()
        {
            var sql = new SqlDataAccess(_configuration);

            var output = sql.LoadData<ProjectModel, dynamic>("spProjects_GetAll", new {}, ConnectionStringName);

            return output;
        }

        public void UpdateProject(ProjectModel project)
        {
            var sql = new SqlDataAccess(_configuration);

            var parameters = new { project.Id, project.Name, project.ProjectPrefix };

            sql.SaveData<dynamic>("spProjects_Update", parameters, ConnectionStringName);
        }
    }
}
