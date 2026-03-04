using APPLICATION.DTOs.Webhook;
using APPLICATION.Services.Webhook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Interfaces.Webhook
{
    public interface IPaymentWebhookService
    {
        Task<PaymentProcessingResult> ProcessPaymentAsync(PaymentWebhookDto dto, CancellationToken ct = default);
    }
}
