using APPLICATION.DTOs.Payments.Request;
using APPLICATION.DTOs.Payments.Response;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Interfaces.Payment
{
    public interface IPaymentsService
    {
        Task<CreatePaymentSessionResponseDto> CreatePaymentSessionAsync(CreatePaymentSessionRequestDto request, CancellationToken ct = default);
        Task HandleProviderWebhookAsync(string provider, string payload, IHeaderDictionary headers, CancellationToken ct = default);
        Task<PaymentResponseDto?> GetPaymentAsync(Guid paymentId, CancellationToken ct = default);
    }
}
