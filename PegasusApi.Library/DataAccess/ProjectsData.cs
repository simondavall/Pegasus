using System.Collections.Generic;
using System.Linq;
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

        public void AddProject(ProjectModel project)
        {
            var parameters = new { project.Name, project.ProjectPrefix };
            _dataAccess.SaveData<dynamic>("spProjects_Add", parameters, ConnectionStringName);
        }

        public void DeleteProject(int id)
        {
            _dataAccess.SaveData<dynamic>("spProjects_Delete", new { id }, ConnectionStringName);
        }

        public ProjectModel GetProject(int id)
        {
            var output = _dataAccess.LoadData<ProjectModel, dynamic>("spProjects_Get", new { id }, ConnectionStringName);
            return output.FirstOrDefault();
        }

        public List<ProjectModel> GetProjects()
        {
            var output = _dataAccess.LoadData<ProjectModel, dynamic>("spProjects_GetAll", new {}, ConnectionStringName);
            return output;
        }

        public void UpdateProject(ProjectModel project)
        {
            var parameters = new { project.Id, project.Name, project.ProjectPrefix };
            _dataAccess.SaveData<dynamic>("spProjects_Update", parameters, ConnectionStringName);
        }
    }
}
