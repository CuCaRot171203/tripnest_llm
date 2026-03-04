using DOMAIN;
using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Repositories.Worker
{
    public class NotificationsRepository : INotificationsRepository
    {
        private readonly TripnestDbContext _db;

        public NotificationsRepository(TripnestDbContext db)
        {
            _db = db;
        }

        public Task AddAsync(Notifications entity, CancellationToken ct = default)
        {
            _db.Notifications.Add(entity);
            return Task.CompletedTask;
        }

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return _db.SaveChangesAsync(ct);
        }
    }
}
