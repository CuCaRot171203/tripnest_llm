using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Interfaces.External
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string plainTextBody, CancellationToken ct = default);
    }
}
