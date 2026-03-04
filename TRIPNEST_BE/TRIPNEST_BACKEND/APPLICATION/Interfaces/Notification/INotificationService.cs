using APPLICATION.DTOs.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Interfaces.Notification
{
    public interface INotificationService
    {
        Task<PagedResultDto<NotificationDto>> GetNotificationsAsync(NotificationQueryDto query, Guid currentUserId, CancellationToken ct = default);
        Task<NotificationDto?> MarkAsReadAsync(Guid notificationId, Guid currentUserId, CancellationToken ct = default);
        Task<NotificationDto> CreateNotificationAsync(CreateNotificationRequestDto request, CancellationToken ct = default);
    }
}
