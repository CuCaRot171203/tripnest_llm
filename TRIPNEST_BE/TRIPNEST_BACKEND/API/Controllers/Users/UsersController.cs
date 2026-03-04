using APPLICATION.DTOs.Users.Request;
using APPLICATION.DTOs.Users.Response;
using APPLICATION.Interfaces.Users;
using APPLICATION.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUsersService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        private bool TryGetCurrentUserId(out Guid userId)
        {
            userId = Guid.Empty;
            var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? User.FindFirst("sub")?.Value
                      ?? User.FindFirst("user_id")?.Value;

            if (string.IsNullOrWhiteSpace(sub)) return false;
            return Guid.TryParse(sub, out userId);
        }

        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserResponse>> GetCurrentUser(CancellationToken cancellationToken)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized();
            }

            try
            {
                var user = await _userService.GetCurrentUserAsync(userId);
                return Ok(user);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPut("me")]
        [Authorize]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateCurrentUser(
            [FromBody] UpdateProfileRequest request,
            CancellationToken cancellationToken)
        {
            if(!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized();
            }

            try
            {
                var updated = await _userService.UpdateCurrentUserAsync(userId, request);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid update profile request for user {UserId}", userId);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPublicProfile([FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var user = await _userService.GetPublicProfileAsync(id);
            if(user == null)
            {
                return NotFound();
            }
            return Ok(new { user });
        }

        [HttpPost("invite")]
        [Authorize(Roles = "CompanyManager,Admin")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Invite([FromBody] InviteUserRequest request)
        {
            if (!TryGetCurrentUserId(out var callerId))
            {
                return Unauthorized();
            }
            if(request == null)
            {
                return BadRequest(new { message = "Invalid request body" });
            }
            if(string.IsNullOrWhiteSpace(request.Email) || request.CompanyId == Guid.Empty)
            {
                return BadRequest(new { message = "Email, CompanyId and Role are required" });
            }
            try
            {
                await _userService.InviteUserAsync(callerId, request);
                return Accepted(new { message = "Invite processed" });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch(ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id:guid}/bookings")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<BookingSummaryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetBookings([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            if (!TryGetCurrentUserId(out var callerId))
            {
                return Unauthorized();
            }

            var isOwner = callerId == id;
            var hasPrivilegedRole = User.IsInRole("Admin") || User.IsInRole("CompanyManager");
            if (!isOwner && !hasPrivilegedRole)
            {
                return Forbid();
            }

            var bookings = await _userService.GetUserBookingsAsync(callerId, id);
            return Ok(bookings);
        }
    }
}
