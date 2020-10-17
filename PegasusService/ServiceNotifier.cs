using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PegasusService.Extensions;

namespace PegasusService
{
    class ServiceNotifier
    {
        private readonly IConfiguration _configuration;
        private DateTime _lastNotified;
        private readonly int _notificationInterval;

        private const int OneHour = 3_600_000;
        private const int NotificationDelayFallback = OneHour;

        public ServiceNotifier(IConfiguration configuration)
        {
            _configuration = configuration;
            _lastNotified = DateTime.Now;
            _notificationInterval = SetNotificationDelay();
        }

        public void Notify(ILogger<Worker> logger)
        {
            if (IsNotificationDue(_notificationInterval, _lastNotified))
            {
                _lastNotified = DateTime.Now;
                logger.LogInformation("PegasusService is running...");
            }
        }

        private static bool IsNotificationDue(int notificationInterval, DateTime lastNotified)
        {
            var interval = TimeSpan.FromMilliseconds(notificationInterval);
            var sinceLastNotification = DateTime.Now - lastNotified;
            return sinceLastNotification > interval;
        }

        private int SetNotificationDelay()
        {
            return _configuration.FromConfig("KeepAlive:NotificationDelay", NotificationDelayFallback);
        }
    }
}
