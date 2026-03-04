using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Payments.Request
{
    public class CreatePaymentSessionRequestDto
    {
        public Guid BookingId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string ReturnUrl { get; set; } = "";
        // Optional: which provider (stripe/vnpay)
        public string? Provider { get; set; }
    }
}
