using DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Interfaces.Notification
{
    public interface INotificationRepository
    {
        Task AddAsync(Notifications entity, CancellationToken ct = default);
        Task<Notifications?> GetByIdAsync(Guid notificationId, CancellationToken ct = default);
        Task UpdateAsync(Notifications entity, CancellationToken ct = default);
        Task<(List<Notifications> items, int total)> GetByUserAsync(
            Guid userId, bool? isRead, string? type, int page, 
            int pageSize, CancellationToken ct = default);
    }
}
