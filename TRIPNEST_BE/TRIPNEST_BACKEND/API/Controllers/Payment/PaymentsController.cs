using APPLICATION.DTOs.Payments.Request;
using APPLICATION.DTOs.Payments.Response;
using APPLICATION.Interfaces.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Payment
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentsService _paymentsService;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(IPaymentsService paymentsService, ILogger<PaymentsController> logger)
        {
            _paymentsService = paymentsService;
            _logger = logger;
        }

        [HttpPost("create-session")]
        [Authorize]
        public async Task<ActionResult<CreatePaymentSessionResponseDto>> CreateSession([FromBody] CreatePaymentSessionRequestDto req, CancellationToken ct)
        {
            // Optionally check that the current user is owner of booking, etc.
            var res = await _paymentsService.CreatePaymentSessionAsync(req, ct);
            return Ok(res);
        }

        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> Webhook(CancellationToken ct)
        {
            // Read raw body
            using var reader = new StreamReader(Request.Body);
            var payload = await reader.ReadToEndAsync(ct);

            // Extract provider from header or query (for example: X-Provider header or query string)
            var provider = Request.Headers.ContainsKey("X-Payment-Provider")
                ? Request.Headers["X-Payment-Provider"].ToString()
                : Request.Query["provider"].ToString();

            if (string.IsNullOrEmpty(provider))
            {
                _logger.LogWarning("Webhook call without provider specified");
                // If you prefer, infer provider by signature header names (e.g., Stripe: Stripe-Signature)
                if (Request.Headers.ContainsKey("Stripe-Signature")) provider = "stripe";
                else if (Request.Headers.ContainsKey("vnp_TmnCode")) provider = "vnpay";
                else provider = "unknown";
            }

            try
            {
                await _paymentsService.HandleProviderWebhookAsync(provider!, payload, Request.Headers, ct);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing webhook");
                // Respond 200 or 400 depending on whether you want provider to retry
                return BadRequest();
            }
        }

        [HttpGet("{paymentId:guid}")]
        [Authorize]
        public async Task<ActionResult<PaymentResponseDto>> GetPayment(Guid paymentId, CancellationToken ct)
        {
            var p = await _paymentsService.GetPaymentAsync(paymentId, ct);
            if (p == null) return NotFound();
            // Optionally check that current user owns the payment / booking or has admin rights
            return Ok(p);
        }
    }
}
