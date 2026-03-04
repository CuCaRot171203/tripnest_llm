using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Webhook
{
    public class PaymentWebhookDto
    {
        public string? Provider { get; set; }

        public string ProviderRef { get; set; } = null!;

        public string? BookingRef { get; set; }

        public Guid? BookingId { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; } = "USD";

        public string Status { get; set; } = null!;

        public DateTime? PaidAt { get; set; }

        public IDictionary<string, string>? Metadata { get; set; }
    }
}
