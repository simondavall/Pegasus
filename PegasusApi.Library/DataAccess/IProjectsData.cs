using System.Collections.Generic;
using PegasusApi.Library.Models;

namespace PegasusApi.Library.DataAccess
{
    public interface IProjectsData
    {
        void AddProject(ProjectModel project);
        void DeleteProject(int id);
        ProjectModel GetProject(int id);
        List<ProjectModel> GetProjects();
        void UpdateProject(ProjectModel project);
    }
}