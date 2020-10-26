using System.Net.Http;

namespace Pegasus.Library.Api
{
    public interface IApiHelper
    {
        HttpClient ApiClient { get; }
    }
}