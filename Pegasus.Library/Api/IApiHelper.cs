using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Pegasus.Library.Models;

namespace Pegasus.Library.Api
{
    public interface IApiHelper
    {
        Task<AuthenticatedUser> Authenticate(string username, string password);
        HttpClient ApiClient { get; }
        Task<T> GetFromUri<T>(string requestUri);
        Task<List<T>> GetListFromUri<T>(string requestUri);
        Task PostAsync<T>(T model, string requestUri);
    }
}