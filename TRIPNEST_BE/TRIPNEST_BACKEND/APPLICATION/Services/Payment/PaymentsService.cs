//using APPLICATION.DTOs.Payments.Request;
//using APPLICATION.DTOs.Payments.Response;
//using APPLICATION.Interfaces.Notification;
//using APPLICATION.Interfaces.Payment;
//using DOMAIN;
//using DOMAIN.Models;
//using Microsoft.AspNetCore.SignalR;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace APPLICATION.Services.Payment
//{
//    public class PaymentsService : IPaymentsService
//    {
//        //private readonly IPaymentsRepository _repo;
//        private readonly TripnestDbContext _db;
//        private readonly INotificationSender _notifier; // assume you have this hub
//        private readonly ILogger<PaymentsService> _logger;
    
//        public PaymentsService(
//            //IPaymentsRepository repo,
//            TripnestDbContext db,
//            INotificationSender notifier,
//            ILogger<PaymentsService> logger)
//        {
//            _repo = repo;
//            _db = db;
//            _notifier = notifier;
//            _logger = logger;
//        }

//        public async Task<CreatePaymentSessionResponseDto> CreatePaymentSessionAsync(CreatePaymentSessionRequestDto request, CancellationToken ct = default)
//        {
//            var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.BookingId == request.BookingId, ct);
//            if (booking == null)
//                throw new KeyNotFoundException("Booking not found.");

//            var paymentEntity = new Payments
//            {
//                BookingId = request.BookingId,
//                Provider = request.Provider,
//                Amount = request.Amount,
//                Currency = request.Currency,
//                Status = "Pending",
//                CreatedAt = DateTime.UtcNow
//            };

//            paymentEntity = await _repo.CreateAsync(paymentEntity, ct);

//            // TODO: Replace with real provider integration
//            var fakeProviderSessionUrl = $"https://payments.example.com/checkout/{paymentEntity.PaymentId}";
//            var fakeProviderSessionId = $"session_{paymentEntity.PaymentId:N}";

//            return new CreatePaymentSessionResponseDto
//            {
//                PaymentId = paymentEntity.PaymentId,
//                ProviderSessionUrl = fakeProviderSessionUrl,
//                ProviderSessionId = fakeProviderSessionId
//            };
//        }

//        public async Task HandleProviderWebhookAsync(string provider, string payload, Microsoft.AspNetCore.Http.IHeaderDictionary headers, CancellationToken ct = default)
//        {
//            _logger.LogInformation("Handling webhook for provider {Provider}", provider);

//            // TODO: verify signature/header and parse provider-specific payload to set these fields:
//            string providerRef = ""; // <-- extract from payload
//            string status = "";      // <-- "succeeded", "failed", "paid", etc.
//            Guid? paymentId = null;  // <-- if provider includes our paymentId in metadata

//            Payments? payment = null;
//            if (paymentId.HasValue)
//            {
//                payment = await _repo.GetByIdAsync(paymentId.Value, ct);
//            }
//            else if (!string.IsNullOrEmpty(providerRef))
//            {
//                payment = await _repo.GetByProviderRefAsync(provider, providerRef, ct);
//            }
//            if (payment == null)
//            {
//                _logger.LogWarning("Payment not found for webhook providerRef={ProviderRef}", providerRef);
//                return;
//            }

//            var beforeJson = System.Text.Json.JsonSerializer.Serialize(payment);

//            if (status == "succeeded" || status == "paid")
//            {
//                payment.Status = "Succeeded";
//                payment.PaidAt = DateTime.UtcNow;
//                payment.Provider = provider;
//                payment.ProviderRef = providerRef;
//            }
//            else if (status == "failed" || status == "cancelled")
//            {
//                payment.Status = "Failed";
//                payment.Provider = provider;
//                payment.ProviderRef = providerRef;
//            }
//            else
//            {
//                payment.Status = status;
//            }

//            await _repo.UpdateAsync(payment, ct);

//            if (payment.Status == "Succeeded" && payment.BookingId.HasValue)
//            {
//                var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.BookingId == payment.BookingId.Value, ct);
//                if (booking != null)
//                {
//                    var bookingBefore = System.Text.Json.JsonSerializer.Serialize(booking);
//                    booking.Status = "Paid";
//                    booking.UpdatedAt = DateTime.UtcNow;
//                    _db.Bookings.Update(booking);
//                    await _db.SaveChangesAsync(ct);

//                    var audit = new Auditlogs
//                    {
//                        AuditId = Guid.NewGuid(),
//                        EntityType = nameof(Bookings),
//                        EntityId = booking.BookingId.ToString(),
//                        Action = "PaymentSucceeded",
//                        BeforeJson = bookingBefore,
//                        AfterJson = System.Text.Json.JsonSerializer.Serialize(booking),
//                        PerformedAt = DateTime.UtcNow,
//                        PerformedBy = null
//                    };
//                    _db.Auditlogs.Add(audit);
//                    await _db.SaveChangesAsync(ct);

//                    var notification = new Notifications
//                    {
//                        NotificationId = Guid.NewGuid(),
//                        UserId = booking.UserId,
//                        Type = "PaymentSucceeded",
//                        Payload = $"Payment for booking {booking.BookingId} succeeded.",
//                        IsRead = false,
//                        CreatedAt = DateTime.UtcNow
//                    };
//                    _db.Notifications.Add(notification);
//                    await _db.SaveChangesAsync(ct);

//                    try
//                    {
//                        await _notifier.SendToUserAsync(
//                            booking.UserId.ToString(),
//                            "payment:updated",
//                            new
//                            {
//                                bookingId = booking.BookingId,
//                                paymentId = payment.PaymentId,
//                                status = payment.Status
//                            },
//                            ct);
//                    }
//                    catch (Exception ex)
//                    {
//                        _logger.LogWarning(ex, "Failed to send notification");
//                    }
//                }
//            }
//            var auditPayment = new Auditlogs
//            {
//                AuditId = Guid.NewGuid(),
//                EntityType = nameof(Payments),
//                EntityId = payment.PaymentId.ToString(),
//                Action = "PaymentWebhookProcessed",
//                BeforeJson = beforeJson,
//                AfterJson = System.Text.Json.JsonSerializer.Serialize(payment),
//                PerformedAt = DateTime.UtcNow
//            };
//            _db.Auditlogs.Add(auditPayment);
//            await _db.SaveChangesAsync(ct);
//        }

//        public async Task<PaymentResponseDto?> GetPaymentAsync(Guid paymentId, CancellationToken ct = default)
//        {
//            var p = await _repo.GetByIdAsync(paymentId, ct);
//            if (p == null) return null;
//            return new PaymentResponseDto
//            {
//                PaymentId = p.PaymentId,
//                BookingId = p.BookingId,
//                Provider = p.Provider,
//                ProviderRef = p.ProviderRef,
//                Amount = p.Amount,
//                Currency = p.Currency,
//                Status = p.Status,
//                PaidAt = p.PaidAt,
//                CreatedAt = p.CreatedAt
//            };
//        }
//    }
//}
