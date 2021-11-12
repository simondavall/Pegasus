using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PegasusService.Extensions;

namespace PegasusService
{
    public class Worker : BackgroundService
    {
        private HttpClient _client;
        private readonly IConfiguration _configuration;
        private KeepAliveService _keepAliveService;
        private readonly ILogger<Worker> _logger;
        private readonly ServiceNotifier _serviceNotifier;
        private int _taskDelay;

        private const int FiveMinutes = 300_000;
        private const int TaskDelayFallback = FiveMinutes;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _serviceNotifier = new ServiceNotifier(configuration);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _serviceNotifier.Notify(_logger);
                await _keepAliveService.Execute(stoppingToken);
                await Task.Delay(_taskDelay, stoppingToken);
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var httpClientHandler = new HttpClientHandler
            {
                // bypasses ssl certificate authentication. Only use locally.
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            _client = new HttpClient(httpClientHandler);
            _keepAliveService = new KeepAliveService(_client, _configuration, _logger);
            _taskDelay = SetTaskDelay();
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _client.Dispose();
            return base.StopAsync(cancellationToken);
        }

        private int SetTaskDelay()
        {
            return _configuration.FromConfig("KeepAlive:TaskDelay", TaskDelayFallback);
        }
    }
}
