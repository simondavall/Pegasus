using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pegasus.Library.Models;

namespace Pegasus.Library.Api
{
    public class ProjectsEndpoint : IProjectsEndpoint
    {
        private readonly IApiHelper _apiHelper;

        public ProjectsEndpoint(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task AddProject(ProjectModel project)
        {
            await _apiHelper.PostAsync(project, "api/Project/AddProject");
        }

        public async Task DeleteProject(int id)
        {
            using (var response = await _apiHelper.ApiClient.DeleteAsync($"api/Project/DeleteProject/{id}"))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task<ProjectModel> GetProject(int id)
        {
            return await _apiHelper.GetFromUri<ProjectModel>($"api/Project/GetProject/{id}");
        }

        public async Task<List<ProjectModel>> GetAllProjects()
        {
            return await _apiHelper.GetListFromUri<ProjectModel>("api/Project/GetAllProjects");
        }

        public async Task UpdateProject(ProjectModel project)
        {
            await _apiHelper.PostAsync(project, "api/Project/UpdateProject");
        }
    }
}