using APPLICATION.DTOs.Webhook;
using APPLICATION.Interfaces.Webhook;
using APPLICATION.Services.Webhook;
using INFRASTRUCTURE.Interfaces.Webhook;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Webhook
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhooksController : ControllerBase
    {
        private readonly IPaymentWebhookService _paymentService;
        private readonly IMapsCostService _mapsCostService;
        private readonly ILogger<WebhooksController> _logger;

        public WebhooksController(IPaymentWebhookService paymentService, IMapsCostService mapsCostService, ILogger<WebhooksController> logger)
        {
            _paymentService = paymentService;
            _mapsCostService = mapsCostService;
            _logger = logger;
        }

        [HttpPost("payment")]
        public async Task<IActionResult> Payment(
            [FromBody] PaymentWebhookDto dto, 
            CancellationToken ct)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new { message = "Empty body" });
                }

                var result = await _paymentService
                    .ProcessPaymentAsync(dto, ct);

                return result.Type switch
                {
                    PaymentProcessingResultType
                    .InvalidPayload 
                        => BadRequest(
                            new { 
                                message = result.Message 
                                ?? "Invalid payload" }),

                    PaymentProcessingResultType
                    .BookingNotFound 
                        => NotFound(
                            new { 
                                message = result.Message 
                                ?? "Booking not found" }),

                    PaymentProcessingResultType.AlreadyProcessed => Ok(new { message = result.Message ?? "Already processed", paymentId = result.PaymentEntity?.PaymentId }),

                    PaymentProcessingResultType.Updated => Ok(new { message = "Payment updated", paymentId = result.PaymentEntity?.PaymentId }),

                    PaymentProcessingResultType.Created => CreatedAtAction(nameof(GetPaymentById), new { id = result.PaymentEntity?.PaymentId }, new { message = "Payment created", paymentId = result.PaymentEntity?.PaymentId }),

                    PaymentProcessingResultType.Conflict => Conflict(new { message = result.Message ?? "Conflict saving payment" }),

                    PaymentProcessingResultType.Error => StatusCode(500, new { message = result.Message ?? "Internal error" }),

                    _ => StatusCode(500, new { message = "Unhandled result" })
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in payment webhook");
                return StatusCode(500, new { message = "Unhandled exception", detail = ex.Message });
            }
        }

        [HttpGet("payments/{id:guid}")]
        public async Task<IActionResult> GetPaymentById([FromServices] IPaymentsRepository repo, Guid id, CancellationToken ct)
        {
            var p = await repo.GetByIdAsync(id, ct);
            if (p == null) return NotFound(new { message = "Payment not found" });
            return Ok(new
            {
                p.PaymentId,
                p.BookingId,
                p.Provider,
                p.ProviderRef,
                p.Amount,
                p.Currency,
                p.Status,
                p.PaidAt,
                p.CreatedAt
            });
        }

        [HttpPost("maps-cost")]
        public async Task<IActionResult> MapsCost([FromBody] MapsCostWebhookDto dto, CancellationToken ct)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new { message = "Empty body" });
                }

                // Very simple validation: require at least PropertyId or Cost
                if (dto.PropertyId == null && dto.Cost == null)
                {
                    return BadRequest(new { message = "Missing required fields. Provide at least PropertyId or Cost." });
                }

                var processed = await _mapsCostService.ProcessMapsCostAsync(dto, ct);
                if (processed)
                {
                    return Ok(new { message = "Maps cost processed" });
                }
                else
                {
                    return Accepted(new { message = "Maps cost accepted for async processing" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in maps-cost webhook");
                return StatusCode(500, new { message = "Unhandled exception", detail = ex.Message });
            }
        }
    }
}
