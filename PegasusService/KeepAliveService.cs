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
            var configSites = _configuration["KeepAlive:Sites"];
            if (string.IsNullOrEmpty(configSites))
            {
                _logger.LogError("Cannot find sites in config for key: 'KeepAlive:Sites'");
                return;
            }

            var sites = configSites.Split(";", StringSplitOptions.RemoveEmptyEntries);
            foreach (var site in sites)
            {
                try
                {
                    _logger.LogDebug("Worker running at: {Time}, Site: {Site}", DateTimeOffset.Now, site);
                    var result = await KeepAlive(site, stoppingToken);
                    _logger.LogDebug("Result for Site: {Site}, Success: {Status}", site, result);

                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Pegasus Service running at: {Time}, Error: {Message}", DateTimeOffset.Now, e.Message);
                }
            }
        }

        public async Task<bool> KeepAlive(string website, CancellationToken stoppingToken)
        {
            var response = await _client.GetAsync(website, stoppingToken);
            return response.IsSuccessStatusCode;
        }
    }
}
