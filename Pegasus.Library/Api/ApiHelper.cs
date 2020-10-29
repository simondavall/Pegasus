using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
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
            var httpClientHandler = new HttpClientHandler
            {
                // This bypasses the SSL certificate check. Prevents the invalid license error message
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            _apiClient = new HttpClient(httpClientHandler)
            {
                BaseAddress = new Uri(apiRoot)
            };
            
            
            _apiClient.DefaultRequestHeaders.Accept.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<T> GetFromUri<T>(string requestUri)
        {
            using (var response = await ApiClient.GetAsync(requestUri))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.ReasonPhrase);
                }
                var result = await response.Content.ReadAsAsync<T>();
                return result;
            }
        }

        public async Task<List<T>> GetListFromUri<T>(string requestUri)
        {
            using (var response = await ApiClient.GetAsync(requestUri))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.ReasonPhrase);
                }
                var result = await response.Content.ReadAsAsync<List<T>>();
                return result;
            }
        }

        public async Task PostAsync<T>(T model, string requestUri)
        {
            HttpContent content = new ObjectContent<T>(model, new JsonMediaTypeFormatter());
            using (var response = await ApiClient.PostAsync(requestUri, content))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
