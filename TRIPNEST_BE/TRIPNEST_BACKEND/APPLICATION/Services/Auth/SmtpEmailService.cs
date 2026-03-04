using APPLICATION.Interfaces.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Services.Auth
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(IConfiguration config, ILogger<SmtpEmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public Task SendPasswordResetEmailAsync(string toEmail, string resetLink, string token)
        {
            // TODO: implement real SMTP / SendGrid send here.
            // For now just log.
            _logger.LogInformation("SendPasswordResetEmail to {Email}: {Link}", toEmail, resetLink);
            return Task.CompletedTask;
        }
    }
}
