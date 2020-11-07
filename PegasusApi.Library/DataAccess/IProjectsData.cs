using System.Collections.Generic;
using System.Threading.Tasks;
using PegasusApi.Library.Models;

namespace PegasusApi.Library.DataAccess
{
    public interface IProjectsData
    {
        Task AddProject(ProjectModel project);
        Task DeleteProject(int id);
        Task<ProjectModel> GetProject(int id);
        Task<List<ProjectModel>> GetProjects();
        Task UpdateProject(ProjectModel project);
    }
}