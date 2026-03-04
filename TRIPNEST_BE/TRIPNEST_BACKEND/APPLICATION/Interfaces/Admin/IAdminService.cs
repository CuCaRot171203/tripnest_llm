using APPLICATION.DTOs.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Interfaces.Admin
{
    public interface IAdminService
    {
        Task<PagedResultDto<AuditLogDto>> GetAuditLogsAsync(AuditLogQueryDto query, CancellationToken ct = default);
        Task<MetricsDto> GetMetricsAsync(CancellationToken ct = default);
        Task TriggerReindexAsync(CancellationToken ct = default); // triggers background reindex
    }
}
