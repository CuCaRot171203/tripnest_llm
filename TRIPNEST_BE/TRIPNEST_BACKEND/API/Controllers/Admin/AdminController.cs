using APPLICATION.DTOs.Admin;
using APPLICATION.Interfaces.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _admin;

        public AdminController(IAdminService admin)
        {
            _admin = admin;
        }

        [HttpGet("auditlogs")]
        public async Task<IActionResult> GetAuditLogs([FromQuery] AuditLogQueryDto query, CancellationToken ct)
        {
            try
            {
                var result = await _admin.GetAuditLogsAsync(query, ct);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to query audit logs", detail = ex.Message });
            }
        }

        [HttpGet("metrics")]
        public async Task<IActionResult> GetMetrics(CancellationToken ct)
        {
            try
            {
                var m = await _admin.GetMetricsAsync(ct);
                return Ok(m);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to get metrics", detail = ex.Message });
            }
        }

        [HttpPost("reindex/search")]
        public async Task<IActionResult> TriggerReindex(CancellationToken ct)
        {
            try
            {
                await _admin.TriggerReindexAsync(ct);
                return Accepted(new { message = "Reindex started" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to trigger reindex", detail = ex.Message });
            }
        }
    }
}
