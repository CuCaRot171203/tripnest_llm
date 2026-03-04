using APPLICATION.DTOs.Admin;
using APPLICATION.Interfaces.Admin;
using INFRASTRUCTURE.Interfaces.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Services.Admin
{
    public class AdminService : IAdminService
    {
        private readonly IAuditLogRepository _auditRepo;
        private readonly IEmbeddingsRepository _embRepo;

        public AdminService(IAuditLogRepository auditRepo, IEmbeddingsRepository embRepo)
        {
            _auditRepo = auditRepo;
            _embRepo = embRepo;
        }

        public async Task<PagedResultDto<AuditLogDto>> GetAuditLogsAsync(
            AuditLogQueryDto query, 
            CancellationToken ct = default)
        {
            var (items, total) = await _auditRepo.QueryAsync(
                query.EntityType,
                query.EntityId,
                query.UserId,
                query.From,
                query.To,
                query.Page,
                query.PageSize,
                query.SortBy,
                query.SortDirection,
                ct);

            var dtoItems = items.Select(m => new AuditLogDto
            {
                AuditId = m.AuditId,
                EntityType = m.EntityType,
                EntityId = m.EntityId,
                Action = m.Action,
                BeforeJson = m.BeforeJson,
                AfterJson = m.AfterJson,
                PerformedBy = m.PerformedBy,
                PerformedAt = m.PerformedAt,
                PerformedByName = m.PerformedByNavigation != null ? m.PerformedByNavigation.Email : null
            }).ToList();

            return new PagedResultDto<AuditLogDto>
            {
                Items = dtoItems,
                Total = total,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<MetricsDto> GetMetricsAsync(CancellationToken ct = default)
        {
            var metrics = new MetricsDto();
            var propsCount = await Task.FromResult(0);
            metrics.Counters["properties_count"] = propsCount;
            metrics.Counters["embeddings_count"] = (await _embRepo.GetByItemTypeAsync("property", ct)).Count;
            return metrics;
        }

        public async Task TriggerReindexAsync(CancellationToken ct = default)
        {
            var embeddings = await _embRepo.GetByItemTypeAsync("property", ct);
            await Task.CompletedTask;
        }
    }
}
