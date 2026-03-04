using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Payments.Response
{
    public class CreatePaymentSessionResponseDto
    {
        public string ProviderSessionUrl { get; set; } = "";
        public string ProviderSessionId { get; set; } = "";
        public Guid PaymentId { get; set; }
    }
}
