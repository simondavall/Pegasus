using System.Collections.Generic;
using System.Threading.Tasks;
using Pegasus.Library.Models;

namespace Pegasus.Library.Api
{
    public interface IProjectsEndpoint
    {
        Task AddProject(ProjectModel project);
        Task DeleteProject(int id);
        Task<ProjectModel> GetProject(int id);
        Task<List<ProjectModel>> GetAllProjects();
        Task UpdateProject(ProjectModel project);

    }
}