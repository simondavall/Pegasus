using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
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

        public async Task Add(ProjectModel project)
        {
            HttpContent content = new ObjectContent<ProjectModel>(project, new JsonMediaTypeFormatter());
            using (var response = await _apiHelper.ApiClient.PostAsync("api/Project/AddProject", content))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task Delete(int id)
        {
            using (var response = await _apiHelper.ApiClient.DeleteAsync($"api/Project/DeleteProject/{id}"))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task<ProjectModel> Get(int id)
        {
            using (var response = await _apiHelper.ApiClient.GetAsync($"api/Project/GetProject/{id}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<ProjectModel>();
                    return result;
                }

                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<List<ProjectModel>> GetAll()
        {
            using (var response = await _apiHelper.ApiClient.GetAsync("api/Project/GetAllProjects"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<List<ProjectModel>>();
                    return result;
                }

                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task Update(ProjectModel project)
        {
            HttpContent content = new ObjectContent<ProjectModel>(project, new JsonMediaTypeFormatter());
            using (var response = await _apiHelper.ApiClient.PostAsync("api/Project/UpdateProject", content))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}