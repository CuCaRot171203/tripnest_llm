using APPLICATION.DTOs.Admin;
using APPLICATION.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Admin
{
    [Route("api/admin/users")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : ControllerBase
    {
        private readonly IAdminUserService _svc;
        private readonly ILogger<AdminUsersController> _logger;

        public AdminUsersController(IAdminUserService svc, ILogger<AdminUsersController> logger)
        {
            _svc = svc;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUsers([FromQuery] int? role, [FromQuery] int? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken ct = default)
        {
            try
            {
                bool? isActive = null;
                if (status.HasValue)
                {
                    if (status != 0 && status != 1)
                    {
                        return BadRequest(
                            new { message = "Invalid status value. Use 1 for active or 0 for inactive." });
                    }
                        

                    isActive = status == 1;
                }

                var result = await _svc.GetUsersAsync(role, isActive, page, pageSize, ct);
                return Ok(result);
            }
            catch (OperationCanceledException) { return BadRequest(new { message = "Request cancelled." }); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetUsers");
                return StatusCode(500, new { message = "Internal server error." });
            }
        }

        [HttpPut("{id:guid}/role")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateRole([FromRoute] Guid id, [FromBody] UpdateUserRoleRequest req, CancellationToken ct = default)
        {
            try
            {
                if (req == null)
                    return BadRequest(new { message = "Request body is required." });
                var updated = await _svc.UpdateUserRoleAsync(id, req.RoleId, ct);
                if (updated == null)
                    return NotFound(new { message = $"User {id} not found." });

                return Ok(updated);
            }
            catch (OperationCanceledException) { return BadRequest(new { message = "Request cancelled." }); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateRole");
                return StatusCode(500, new { message = "Internal server error." });
            }
        }
    }
}
