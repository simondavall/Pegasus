using System.Collections.Generic;
using System.Threading.Tasks;
using Pegasus.Library.Models;

namespace Pegasus.Library.Api
{
    public interface IProjectsEndpoint
    {
        Task Add(ProjectModel project);
        Task Delete(int id);
        Task<ProjectModel> Get(int id);
        Task<List<ProjectModel>> GetAll();
        Task Update(ProjectModel project);

    }
}