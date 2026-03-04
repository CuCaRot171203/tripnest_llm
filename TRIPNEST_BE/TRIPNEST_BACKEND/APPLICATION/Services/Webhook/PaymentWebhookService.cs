using APPLICATION.DTOs.Webhook;
using APPLICATION.Interfaces.Payment;
using APPLICATION.Interfaces.Webhook;
using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.Webhook;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Services.Webhook
{
    public class PaymentWebhookService : IPaymentWebhookService
    {
        private readonly INFRASTRUCTURE.Interfaces.Webhook.IPaymentsRepository _paymentsRepo;
        private readonly IBookingsRepository _bookingsRepo;

        public PaymentWebhookService(INFRASTRUCTURE.Interfaces.Webhook.IPaymentsRepository paymentsRepo, IBookingsRepository bookingsRepo)
        {
            _paymentsRepo = paymentsRepo;
            _bookingsRepo = bookingsRepo;
        }

        public async Task<PaymentProcessingResult> ProcessPaymentAsync(PaymentWebhookDto dto, CancellationToken ct = default)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ProviderRef) || string.IsNullOrWhiteSpace(dto.Status))
            {
                return new PaymentProcessingResult { Type = PaymentProcessingResultType.InvalidPayload, Message = "Missing required fields." };
            }

            var existing = await _paymentsRepo.GetByProviderRefAsync(dto.Provider ?? string.Empty, dto.ProviderRef, ct);
            if (existing != null)
            {
                var changed = false;
                if (existing.Status != dto.Status) { existing.Status = dto.Status; changed = true; }
                if (dto.Amount != 0 && existing.Amount != dto.Amount) { existing.Amount = dto.Amount; changed = true; }
                if (dto.Currency != null && existing.Currency != dto.Currency) { existing.Currency = dto.Currency; changed = true; }
                if (dto.PaidAt.HasValue && existing.PaidAt != dto.PaidAt) { existing.PaidAt = dto.PaidAt; changed = true; }

                if (!changed)
                {
                    return new PaymentProcessingResult { Type = PaymentProcessingResultType.AlreadyProcessed, Message = "Payment already processed.", PaymentEntity = existing };
                }

                try
                {
                    existing.CreatedAt ??= DateTime.UtcNow;
                    await _paymentsRepo.UpdateAsync(existing, ct);
                    await _paymentsRepo.SaveChangesAsync(ct);

                    return new PaymentProcessingResult { Type = PaymentProcessingResultType.Updated, PaymentEntity = existing };
                }
                catch (Exception ex)
                {
                    return new PaymentProcessingResult { Type = PaymentProcessingResultType.Error, Message = ex.Message };
                }
            }

            Bookings? booking = null;
            if (dto.BookingId.HasValue)
            {
                booking = await _bookingsRepo.GetByIdAsync(dto.BookingId.Value, ct);
            }
            
            if (booking == null && !string.IsNullOrWhiteSpace(dto.BookingRef))
            {
                if (Guid.TryParse(dto.BookingRef, out var bId))
                {
                    booking = await _bookingsRepo.GetByIdAsync(bId, ct);
                }
            }

            var payment = new Payments
            {
                PaymentId = Guid.NewGuid(),
                BookingId = booking?.BookingId,
                Provider = dto.Provider,
                ProviderRef = dto.ProviderRef,
                Amount = dto.Amount,
                Currency = dto.Currency,
                Status = dto.Status,
                PaidAt = dto.PaidAt,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                await _paymentsRepo.AddAsync(payment, ct);
                await _paymentsRepo.SaveChangesAsync(ct);

                // Optionally update booking.Status when paid
                if (booking != null && dto.Status.Equals("paid", StringComparison.OrdinalIgnoreCase))
                {
                    booking.Status = "paid";
                    booking.UpdatedAt = DateTime.UtcNow;
                    await _bookingsRepo.UpdateAsync(booking, ct);
                    await _bookingsRepo.SaveChangesAsync(ct);
                }

                return new PaymentProcessingResult { Type = PaymentProcessingResultType.Created, PaymentEntity = payment };
            }
            catch (DbUpdateException dbEx)
            {
                return new PaymentProcessingResult { Type = PaymentProcessingResultType.Conflict, Message = dbEx.Message };
            }
            catch (Exception ex)
            {
                return new PaymentProcessingResult { Type = PaymentProcessingResultType.Error, Message = ex.Message };
            }
        }
    }
}
