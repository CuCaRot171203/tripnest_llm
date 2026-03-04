using APPLICATION.DTOs.Auth;
using APPLICATION.DTOs.Auth.Request;
using APPLICATION.DTOs.Auth.Response;
using APPLICATION.Interfaces.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IAuthService auth, ILogger<AuthController> logger)
        {
            _auth = auth;
            _logger = logger;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequest req,
            CancellationToken cancellationToken)
        {
            try
            {
                var res = await _auth.RegisterAsync(req);
                return Created(string.Empty, res);
            }
            catch(ApplicationException ex)
            {
                _logger.LogWarning(ex, "Register failed for {Email}", req?.Email);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest req,
            CancellationToken cancellationToken
        )
        {
            try
            {
                var res = await _auth.LoginAsync(req);
                return Ok(res);
            }
            catch (ApplicationException ex)
            {
                _logger.LogInformation(ex, "Login failed for {Username}", req?.Email);
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Refresh(
            [FromBody] RefreshRequest req,
            CancellationToken cancellationToken)
        {
            try
            {
                var res = await _auth.RefreshTokenAsync(req);
                return Ok(res);
            }
            catch (ApplicationException ex)
            {
                _logger.LogInformation(ex, "Refresh token failed");
                return Unauthorized(new {message = ex.Message});
            }
        }

        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout(
            [FromBody] LogoutRequest req,
            CancellationToken cancellationToken)
        {
            // Validate request
            if (req == null || string.IsNullOrEmpty(req.RefreshToken))
            {
                return BadRequest(new { message = "Invalid logout request." });
            }

            // Get user ID from claims
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                _logger.LogWarning("Logout attempted but user id claim missing or invalid.");
                return Unauthorized(new { message = "Invalid user." });
            }

            try
            {
                await _auth.LogoutAsync(req, userId);
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                _logger.LogInformation(ex, "Logout failed for user {UserId}", userId);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during logout for user {UserId}", userId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ForgotPassword(
            [FromBody] ForgotPasswordRequest req,
            CancellationToken cancellationToken)
        {
            await _auth.ForgotPasswordAsync(req);
            return Ok(new { message = "If the email exists a reset link will be sent." });
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword(
            [FromBody] ResetPasswordRequest req,
            CancellationToken cancellationToken)
        {
            try
            {
                await _auth.ResetPasswordASync(req);
                return Ok(new { message = "Password reset successful." });
            }
            catch (ApplicationException ex)
            {
                _logger.LogInformation(ex, "Reset password failed for token {Token}", req?.Token);
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
