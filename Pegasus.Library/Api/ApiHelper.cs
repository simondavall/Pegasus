using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Pegasus.Library.Models;

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

        public async Task<AuthenticatedUser> Authenticate(UserCredentials credentials)
        {
            var data = new FormUrlEncodedContent(
                new[]
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", credentials?.Username),
                    new KeyValuePair<string, string>("password", credentials?.Password)
                });

            using (var response = await ApiClient.PostAsync("/token", data))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<AuthenticatedUser>();
                    result.Succeeded = true;
                    return result;
                }
                else
                {
                    var result = new AuthenticatedUser
                    {
                        Succeeded = false,
                        FailedReason = response.ReasonPhrase
                    };
                    return result;
                }
            }
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

        public async Task<T> PostAsync<T>(T model, string requestUri)
        {
            HttpContent content = new ObjectContent<T>(model, new JsonMediaTypeFormatter());
            using (var response = await ApiClient.PostAsync(requestUri, content))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.ReasonPhrase);
                }
                var result = await response.Content.ReadAsAsync<T>();
                return result;
            }
        }
    }
}
