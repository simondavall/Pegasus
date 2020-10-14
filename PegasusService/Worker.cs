using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PegasusService
{
    public class Worker : BackgroundService
    {
        private HttpClient _client;
        private readonly IConfiguration _configuration;
        private KeepAliveService _keepAliveService;
        private readonly ILogger<Worker> _logger;
        private int _taskDelay;

        private const int FiveMinutes = 300000;
        private const int TaskDelayFallback = FiveMinutes;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _keepAliveService.Execute(stoppingToken);
                await Task.Delay(_taskDelay, stoppingToken);
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _client = new HttpClient();
            _keepAliveService = new KeepAliveService(_client, _configuration, _logger);
            if (int.TryParse(_configuration[$"KeepAlive:TaskDelay"], out _taskDelay) == false)
                _taskDelay = TaskDelayFallback;
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _client.Dispose();
            return base.StopAsync(cancellationToken);
        }

    }
}
