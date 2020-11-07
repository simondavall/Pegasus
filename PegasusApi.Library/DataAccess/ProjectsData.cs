using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PegasusApi.Library.Models;

namespace PegasusApi.Library.DataAccess
{
    public class ProjectsData : IProjectsData
    {
        private readonly IDataAccess _dataAccess;
        private const string ConnectionStringName = "Pegasus";

        public ProjectsData(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public  async Task AddProject(ProjectModel project)
        {
            var parameters = new { project.Name, project.ProjectPrefix };
            await _dataAccess.SaveDataAsync<dynamic>("spProjects_Add", parameters, ConnectionStringName);
        }

        public async Task DeleteProject(int id)
        {
            await _dataAccess.SaveDataAsync<dynamic>("spProjects_Delete", new { id }, ConnectionStringName);
        }

        public async Task<ProjectModel> GetProject(int id)
        {
            var output = await _dataAccess.LoadDataAsync<ProjectModel, dynamic>("spProjects_Get", new { id }, ConnectionStringName);
            return output.FirstOrDefault();
        }

        public async Task<List<ProjectModel>> GetProjects()
        {
            var output = await _dataAccess.LoadDataAsync<ProjectModel, dynamic>("spProjects_GetAll", new {}, ConnectionStringName);
            return output;
        }

        public async Task UpdateProject(ProjectModel project)
        {
            var parameters = new { project.Id, project.Name, project.ProjectPrefix };
            await _dataAccess.SaveDataAsync<dynamic>("spProjects_Update", parameters, ConnectionStringName);
        }
    }
}
