using APPLICATION.DTOs.Worker;
using APPLICATION.Interfaces.External;
using APPLICATION.Interfaces.Worker;
using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.Worker;
using Microsoft.Extensions.Logging;

namespace APPLICATION.Services.Worker
{
    public class WorkerService : IWorkerService
    {
        private readonly IPropertiesRepository _propertiesRepo;
        private readonly IEmbeddingsRepository _embRepo;
        private readonly IBookingsRepository _bookingsRepo;
        private readonly INotificationsRepository _notificationsRepo;
        private readonly IVectorService _vectorService;
        private readonly IEmailService _emailService;
        private readonly ILogger<WorkerService> _logger;

        public WorkerService(
            IPropertiesRepository propertiesRepo, 
            IEmbeddingsRepository embRepo, 
            IBookingsRepository bookingsRepo, 
            INotificationsRepository notificationsRepo, 
            IVectorService vectorService, 
            IEmailService emailService, 
            ILogger<WorkerService> logger)
        {
            _propertiesRepo = propertiesRepo;
            _embRepo = embRepo;
            _bookingsRepo = bookingsRepo;
            _notificationsRepo = notificationsRepo;
            _vectorService = vectorService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<IndexPropertyResponseDto> IndexPropertyAsync(
            long propertyId, 
            bool forceReindex = false, 
            CancellationToken ct = default)
        {
            var prop = await _propertiesRepo.GetByIdAsync(propertyId, ct);
            if (prop == null)
            {
                return new IndexPropertyResponseDto
                {
                    PropertyId = propertyId,
                    Indexed = false,
                    Message = "Property not found",
                    ProcessedAt = DateTime.UtcNow
                };
            }

            var text = $"{prop.TitleEn}\n{prop.TitleVi}\n{prop.DescriptionEn}\n{prop.DescriptionVi}\n{prop.AddressFormatted}\n{prop.City}\n{prop.Province}";

            var (vectorRef, vectorBlob) = await _vectorService.CreateEmbeddingAsync(text ?? string.Empty, ct);

            if (vectorRef == null && vectorBlob == null)
            {
                _logger.LogWarning("Vector service returned null for property {PropertyId}", propertyId);
                return new IndexPropertyResponseDto
                {
                    PropertyId = propertyId,
                    Indexed = false,
                    Message = "Failed to create embedding",
                    ProcessedAt = DateTime.UtcNow
                };
            }

            var itemType = "property";
            var itemId = propertyId.ToString();

            var existing = await _embRepo.GetByItemAsync(itemType, itemId, ct);

            if (existing == null)
            {
                var newEmb = new Embeddings
                {
                    ItemType = itemType,
                    ItemId = itemId,
                    VectorRef = vectorRef,
                    VectorBlob = vectorBlob,
                    UpdatedAt = DateTime.UtcNow
                };
                await _embRepo.AddAsync(newEmb, ct);
            }
            else
            {
                existing.VectorRef = vectorRef;
                existing.VectorBlob = vectorBlob;
                existing.UpdatedAt = DateTime.UtcNow;
                await _embRepo.UpdateAsync(existing, ct);
            }

            await _embRepo.SaveChangesAsync(ct);

            return new IndexPropertyResponseDto
            {
                PropertyId = propertyId,
                Indexed = true,
                Message = "Indexed",
                ProcessedAt = DateTime.UtcNow
            };
        }

        public async Task<SendRemindersResultDto> SendRemindersAsync(int daysAhead = 1, CancellationToken ct = default)
        {
            // Determine date window (UTC)
            var from = DateTime.UtcNow.Date.AddDays(daysAhead); // e.g., tomorrow
            var to = from; // only that day; change if you want range

            var bookings = await _bookingsRepo.GetUpcomingBookingsForRemindersAsync(from, to, ct);

            int remindersCreated = 0;
            int emailsSent = 0;

            foreach (var b in bookings)
            {
                // create notification for user
                var payload = new
                {
                    bookingId = b.BookingId,
                    propertyId = b.PropertyId,
                    checkin = b.CheckinDate.ToString(), // or standard format
                    msg = $"Reminder: your check-in is on {b.CheckinDate.ToString()}"
                };

                var noti = new Notifications
                {
                    NotificationId = Guid.NewGuid(),
                    UserId = b.UserId,
                    Type = "booking_reminder",
                    Payload = System.Text.Json.JsonSerializer.Serialize(payload),
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                await _notificationsRepo.AddAsync(noti, ct);
                remindersCreated++;

                // optionally send email if available and user email present
                if (_emailService != null && b.User?.Email != null)
                {
                    try
                    {
                        var subject = $"Reminder: upcoming check-in on {b.CheckinDate}";
                        var body = $"Hello {b.User.FullName ?? b.User.Email},\n\nThis is a reminder that your booking (#{b.BookingId}) at property {b.Property?.TitleEn ?? b.Property?.TitleVi} has check-in on {b.CheckinDate}.\n\nThanks.";
                        var ok = await _emailService.SendEmailAsync(b.User.Email, subject, body, ct);
                        if (ok) emailsSent++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send reminder email for booking {BookingId}", b.BookingId);
                    }
                }
            }

            await _notificationsRepo.SaveChangesAsync(ct);

            return new SendRemindersResultDto
            {
                RemindersCreated = remindersCreated,
                EmailsSent = emailsSent,
                ProcessedAt = DateTime.UtcNow
            };
        }
    }
}
