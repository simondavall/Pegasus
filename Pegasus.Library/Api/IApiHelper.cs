using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pegasus.Library.Api
{
    public interface IApiHelper
    {
        void AddTokenToHeaders(string token);
        void RemoveTokenFromHeaders();
		HttpClient ApiClient { get; }
        Task<T> GetFromUri<T>(string requestUri);
        Task<List<T>> GetListFromUri<T>(string requestUri);
        Task<T> PostAsync<T>(T model, string requestUri);
    }
}