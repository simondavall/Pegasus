using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PegasusApi.Library.Exceptions;
using PegasusApi.Library.Services.Resources;
using PegasusApi.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace PegasusApi.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IOptions<EmailSenderOptions> optionsAccessor, IConfiguration configuration, ILogger<EmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
            Options = optionsAccessor.Value;
        }

        public EmailSenderOptions Options { get; } //set only via Secret Manager

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(Options.SendGridKey, subject, message, email);
         }

        internal Task Execute(string apiKey, string subject, string message, string email)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                _logger.LogError(Resources.ErrorStrings.NoEmailApiKeyFound);
                throw new EmailApiKeyNotFoundException(Resources.ErrorStrings.NoEmailApiKeyFound);
            }
            
            var client = new SendGridClient(apiKey);
            var fromEmail = _configuration["Email:FromAddress"];
            var sender = _configuration["Email:Sender"];
            var msg = new SendGridMessage
            {
                From = new EmailAddress(fromEmail, sender),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);

            return client.SendEmailAsync(msg);
        }
    }
}
