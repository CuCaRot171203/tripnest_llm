using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Services.Webhook
{
    public enum PaymentProcessingResultType
    {
        Created,
        Updated,
        AlreadyProcessed,
        BookingNotFound,
        InvalidPayload,
        Conflict,
        Error
    }

    public class PaymentProcessingResult
    {
        public PaymentProcessingResultType Type { get; set; }
        public string? Message { get; set; }
        public DOMAIN.Models.Payments? PaymentEntity { get; set; }
    }
}
