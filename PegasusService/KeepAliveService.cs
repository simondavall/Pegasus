using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PegasusService
{
    public class KeepAliveService
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public KeepAliveService(HttpClient client, IConfiguration configuration, ILogger logger)
        {
            _client = client;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task Execute(CancellationToken stoppingToken)
        {
            try
            {
                var sites = _configuration[$"KeepAlive:Sites"].Split(";", StringSplitOptions.RemoveEmptyEntries);

                foreach (var site in sites)
                {
                    var result = await KeepAlive(site, stoppingToken);
#if DEBUG
                    _logger.LogInformation("Debug: Worker running at: {time}, Site: {site}, Success: {status}", DateTimeOffset.Now, site, result);
#endif
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Pegasus Service running at: {time}, Error: {message}", DateTimeOffset.Now, ex.Message);
                if (ex.InnerException != null)
                    _logger.LogInformation("Pegasus Service running at: {time}, Error: {message}", DateTimeOffset.Now, ex.InnerException.Message);
            }
        }

        public async Task<bool> KeepAlive(string website, CancellationToken stoppingToken)
        {
            var response = await _client.GetAsync(website, stoppingToken);
            return response.IsSuccessStatusCode;
        }
    }
}
