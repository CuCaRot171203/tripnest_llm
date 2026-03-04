using APPLICATION.DTOs.Notification;
using APPLICATION.Interfaces.Notification;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers.Notification
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _service;
        private readonly IMapper _mapper;

        public NotificationsController(INotificationService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get(
            [FromQuery] bool? isRead, 
            [FromQuery] string? type, 
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 20, 
            CancellationToken ct = default)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var query = new NotificationQueryDto
            {
                IsRead = isRead,
                Type = type,
                Page = page <= 0 ? 1 : page,
                PageSize = pageSize <= 0 ? 20 : pageSize
            };

            var paged = await _service.GetNotificationsAsync(query, userId, ct);
            return Ok(paged);
        }

        [HttpPut("{id:guid}/read")]
        [Authorize]
        public async Task<IActionResult> MarkRead([FromRoute] Guid id, CancellationToken ct = default)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var updated = await _service.MarkAsReadAsync(id, userId, ct);
            if (updated == null)
            {
                return NotFound();
            }

            return Ok(updated);
        }

        [HttpPost("send")]
        [Authorize(Roles = "Admin,System,Worker")] // or a policy for system-to-system
        public async Task<IActionResult> Send([FromBody] CreateNotificationRequestDto request, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var created = await _service.CreateNotificationAsync(request, ct);
            return CreatedAtAction(nameof(Get), new { id = created.NotificationId }, created);
        }
    }
}
