using DOMAIN;
using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.Notification;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Repositories.Notification
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly TripnestDbContext _db;

        public NotificationRepository(TripnestDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Notifications entity, CancellationToken ct = default)
        {
            _db.Notifications.Add(entity);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<Notifications?> GetByIdAsync(
            Guid notificationId, CancellationToken ct = default)
        {
            return await _db.Notifications
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    n => n.NotificationId 
                    == notificationId, ct);
        }

        public async Task UpdateAsync(Notifications entity, CancellationToken ct = default)
        {
            _db.Notifications.Update(entity);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<(List<Notifications> items, int total)> GetByUserAsync(
            Guid userId, bool? isRead, string? type, 
            int page, int pageSize, CancellationToken ct = default)
        {
            if (page <= 0)
            {
                page = 1;
            }
            if (pageSize <= 0)
            {
                pageSize = 20;
            }

            var query = _db.Notifications
                .Where(n => n.UserId == userId);

            if (isRead.HasValue)
            {
                query = query
                    .Where(n => (n.IsRead ?? false) == isRead.Value);
            }

            if (!string.IsNullOrWhiteSpace(type))
            {
                query = query.Where(n => n.Type == type);
            }

            var total = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, total);
        }
    }
}
