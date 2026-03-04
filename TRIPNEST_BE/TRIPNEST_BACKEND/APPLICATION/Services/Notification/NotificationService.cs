using APPLICATION.DTOs.Notification;
using APPLICATION.Interfaces.Notification;
using AutoMapper;
using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.Notification;
using Microsoft.Extensions.Logging;

namespace APPLICATION.Services.Notification
{
    public class NotificationService : Interfaces.Notification.INotificationService
    {
        private readonly INotificationRepository _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            INotificationRepository repo,
            IMapper mapper,
            ILogger<NotificationService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PagedResultDto<NotificationDto>> GetNotificationsAsync(NotificationQueryDto query, Guid currentUserId, CancellationToken ct = default)
        {
            var (items, total) = await _repo.GetByUserAsync(currentUserId, query.IsRead, query.Type, query.Page, query.PageSize, ct);
            var dtoItems = _mapper.Map<System.Collections.Generic.List<NotificationDto>>(items);

            return new PagedResultDto<NotificationDto>
            {
                Page = query.Page,
                PageSize = query.PageSize,
                Total = total,
                Items = dtoItems
            };
        }

        public async Task<NotificationDto?> MarkAsReadAsync(Guid notificationId, Guid currentUserId, CancellationToken ct = default)
        {
            var notif = await _repo.GetByIdAsync(notificationId, ct);
            if (notif == null || notif.UserId != currentUserId)
            {
                return null;
            }

            if (notif.IsRead != true)
            {
                notif.IsRead = true;
                await _repo.UpdateAsync(notif, ct);
            }

            return _mapper.Map<NotificationDto>(notif);
        }

        public async Task<NotificationDto> CreateNotificationAsync(
            CreateNotificationRequestDto request, 
            CancellationToken ct = default)
        {
            var entity = new Notifications
            {
                NotificationId = Guid.NewGuid(),
                UserId = request.UserId,
                Type = request.Type,
                Payload = request.Payload,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(entity, ct);

            if (!request.SendNow)
            {
                _logger.LogInformation("Notification {NotificationId} created and queued for delivery.", entity.NotificationId);
            }
            else
            {
                _logger.LogInformation("Notification {NotificationId} created and sent immediately.", entity.NotificationId);
            }

            return _mapper.Map<NotificationDto>(entity);
        }
    }
}
