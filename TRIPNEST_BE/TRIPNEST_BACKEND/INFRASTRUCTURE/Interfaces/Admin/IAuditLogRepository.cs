using DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Interfaces.Admin
{
    public interface IAuditLogRepository
    {
        Task<(List<Auditlogs> Items, int Total)> QueryAsync(
            string? entityType,
            string? entityId,
            Guid? performedBy,
            DateTime? from,
            DateTime? to,
            int page,
            int pageSize,
            string sortBy,
            string sortDirection,
            CancellationToken ct = default);
    }
}
