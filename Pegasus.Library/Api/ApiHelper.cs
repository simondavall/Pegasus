using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;

namespace Pegasus.Library.Api
{
    public class ApiHelper : IApiHelper
    {
        private readonly IConfiguration _configuration;
        private HttpClient _apiClient;

        public ApiHelper(IConfiguration configuration)
        {
            _configuration = configuration;
            InitializeClient();
        }

        public HttpClient ApiClient
        {
            get { return _apiClient; }
        }

        private void InitializeClient()
        {
            var apiRoot = _configuration["apiRoot"];

            _apiClient = new HttpClient
            {
                BaseAddress = new Uri(apiRoot)
            };
            _apiClient.DefaultRequestHeaders.Accept.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
