using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Payments.Response
{
    public class PaymentResponseDto
    {
        public Guid PaymentId { get; set; }
        public Guid? BookingId { get; set; }
        public string? Provider { get; set; }
        public string? ProviderRef { get; set; }
        public decimal? Amount { get; set; }
        public string? Currency { get; set; }
        public string? Status { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
