using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Pegasus.Library.Api
{
    public class ApiHelper : IApiHelper
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiHelper(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            InitializeClient();
        }

        public HttpClient ApiClient { get; private set; }

        private void InitializeClient()
        {
            var apiRoot = _configuration["apiRoot"];
            var httpClientHandler = new HttpClientHandler
            {
                // This bypasses the SSL certificate check. Prevents the invalid license error message
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            ApiClient = new HttpClient(httpClientHandler)
            {
                BaseAddress = new Uri(apiRoot)
            };
            
            ApiClient.DefaultRequestHeaders.Accept.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var token = _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            AddTokenToHeaders(token.Result);
        }

        public void AddTokenToHeaders(string token)
        {
            ApiClient.DefaultRequestHeaders.Remove("Authorization");
            ApiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        public void RemoveTokenFromHeaders()
        {
            ApiClient.DefaultRequestHeaders.Remove("Authorization");
        }

        public async Task<T> GetFromUri<T>(string requestUri)
        {
            using (var response = await ApiClient.GetAsync(requestUri))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error accessing {requestUri}. Error: {response.ReasonPhrase}");
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
                    throw new Exception($"Error accessing {requestUri}. Error: {response.ReasonPhrase}");
                }
                var result = await response.Content.ReadAsAsync<List<T>>();
                return result;
            }
        }

        public async Task<T> PostAsync<T>(T model, string requestUri)
        {
            HttpContent content = new ObjectContent<T>(model, new JsonMediaTypeFormatter());
            using (var response = await ApiClient.PostAsync(requestUri, content))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error accessing {requestUri}. Error: {response.ReasonPhrase}");
                }
                var result = await response.Content.ReadAsAsync<T>();
                return result;
            }
        }
    }
}
