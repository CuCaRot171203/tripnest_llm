using DOMAIN;
using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.Admin;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Repositories.Admin
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly TripnestDbContext _db;

        public AuditLogRepository(TripnestDbContext db)
        {
            _db = db;
        }

        public async Task<(List<Auditlogs> Items, int Total)> QueryAsync(
            string? entityType,
            string? entityId,
            Guid? performedBy,
            DateTime? from,
            DateTime? to,
            int page,
            int pageSize,
            string sortBy,
            string sortDirection,
            CancellationToken ct = default)
        {
            var q = _db.Auditlogs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(entityType))
            {
                q = q.Where(x => x.EntityType == entityType);
            }

            if (!string.IsNullOrWhiteSpace(entityId))
            {
                q = q.Where(x => x.EntityId == entityId);
            }

            if (performedBy.HasValue)
            {
                q = q.Where(x => x.PerformedBy == performedBy);
            }

            if (from.HasValue)
            {
                q = q.Where(x => x.PerformedAt >= from.Value);
            }

            if (to.HasValue)
            {
                q = q.Where(x => x.PerformedAt <= to.Value);
            }

            var total = await q.CountAsync(ct);

            var ordered = (sortBy ?? "PerformedAt").ToLower() switch
            {
                "performedat" => sortDirection?.ToLower() == "asc" ? q.OrderBy(x => x.PerformedAt) : q.OrderByDescending(x => x.PerformedAt),
                "entitytype" => sortDirection?.ToLower() == "asc" ? q.OrderBy(x => x.EntityType) : q.OrderByDescending(x => x.EntityType),
                _ => q.OrderByDescending(x => x.PerformedAt)
            };

            var items = await ordered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(a => a.PerformedByNavigation)
                .ToListAsync(ct);

            return (items, total);
        }
    }
}
