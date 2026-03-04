using APPLICATION.Interfaces.External;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Services.External
{
    public class SmtpEmailService : IEmailService, IDisposable
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SmtpEmailService> _logger;
        private readonly SmtpClient _client;
        private readonly string _fromAddress;
        private readonly string? _fromDisplayName;

        public SmtpEmailService(IConfiguration config, ILogger<SmtpEmailService> logger)
        {
            _config = config;
            _logger = logger;

            var host = _config["Smtp:Host"] ?? throw new ArgumentException("Smtp:Host is required in configuration");
            var port = int.TryParse(_config["Smtp:Port"], out var p) ? p : 25;
            var username = _config["Smtp:Username"];
            var password = _config["Smtp:Password"];
            var enableSsl = bool.TryParse(_config["Smtp:EnableSsl"], out var e) && e;

            _fromAddress = _config["Smtp:FromAddress"] ?? username ?? throw new ArgumentException("Smtp:FromAddress or Smtp:Username is required");
            _fromDisplayName = _config["Smtp:FromDisplayName"];

            _client = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 10000
            };

            if (!string.IsNullOrEmpty(username) && password != null)
            {
                _client.Credentials = new NetworkCredential(username, password);
            }
        }

        public async Task<bool> SendEmailAsync(
            string toEmail, 
            string subject, 
            string plainTextBody, 
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
            {
                _logger.LogWarning("SendEmailAsync called with empty toEmail.");
                return false;
            }

            try
            {
                using var message = new MailMessage();
                message.From = new MailAddress(_fromAddress, _fromDisplayName ?? string.Empty);
                message.To.Add(new MailAddress(toEmail));
                message.Subject = subject ?? string.Empty;
                message.Body = plainTextBody ?? string.Empty;
                message.IsBodyHtml = false;

                // SmtpClient.SendMailAsync supports CancellationToken via Registering with token
                using var ctr = ct.Register(() => _logger.LogDebug("SendEmailAsync cancellation requested."));
                await _client.SendMailAsync(message);
                _logger.LogInformation("Email sent to {To}", toEmail);
                return true;
            }
            catch (SmtpException sx)
            {
                _logger.LogError(sx, "SMTP error sending email to {To}", toEmail);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error sending email to {To}", toEmail);
                return false;
            }
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
