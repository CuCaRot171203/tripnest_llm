using APPLICATION.DTOs.Auth.Request;
using APPLICATION.DTOs.Auth.Response;
using APPLICATION.Interfaces.Auth;
using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.Auth;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repo;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthService> _logger;

        private static readonly ConcurrentDictionary<string,
            (Guid userId, DateTime expiresAt)> _passwordResetTokens
            = new ConcurrentDictionary<string, (Guid, DateTime)>();

        public AuthService(
            IAuthRepository repo, 
            ITokenService tokenService, 
            IEmailService emailService, 
            ILogger<AuthService> logger)
        {
            _repo = repo;
            _tokenService = tokenService;
            _emailService = emailService;
            _logger = logger;
        }

        // Method register user
        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            if(request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            var existing = await _repo.GetByEmailAsync(request.Email);
            if (existing != null)
            {
                throw new ApplicationException("Email already in use.");
            }

            var userId = Guid.NewGuid();
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var createDto = new CreateUserDto
            {
                UserId = userId,
                Email = request.Email,
                PasswordHash = passwordHash,
                FullName = request.FullName,
                Phone = request.Phone,
                Locale = request.Locale,
                IsActive = true
            };

            var userEntity = MapCreateUserDtoToEntity(createDto);
            await _repo.CreateUserAsync(userEntity);
            var userDto = MapToUserDto(userEntity);

            // generate tokens
            var accessToken = _tokenService.GenerateAccessToken(userDto);

            // generate tokens
            var refreshDto = _tokenService.GenerateRefreshToken(userEntity.UserId);
            var refreshEntity = MapRefreshDtoToEntity(refreshDto);
            await _repo.CreateRefreshTokenAsync(refreshEntity);

            return new AuthResponse
            {
                User = userDto,
                AccessToken = accessToken,
                RefreshToken = refreshDto.Token
            };
        }

        // Method login
        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            if(request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var userEntity = await _repo.GetByEmailAsync(request.Email);
            if(userEntity == null)
            {
                throw new ApplicationException("Invalid email or password.");
            }

            var ok = await _repo.VerifyCredentialsAsync(request.Email, request.Password);
            if (!ok)
            {
                throw new ApplicationException("Invalid credentials.");
            }

            var userDto = MapToUserDto(userEntity);
            var accessToken = _tokenService.GenerateAccessToken(userDto);

            var refreshDto = _tokenService.GenerateRefreshToken(userEntity.UserId);
            var refreshEntity = MapRefreshDtoToEntity(refreshDto);
            await _repo.CreateRefreshTokenAsync(refreshEntity);

            return new AuthResponse
            {
                User = userDto,
                AccessToken = accessToken,
                RefreshToken = refreshDto.Token
            };
        }
    
        // Method get refresh token
        public async Task<AuthResponse> RefreshTokenAsync(RefreshRequest request)
        {
            if(request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var existing = await _repo.GetRefreshTokenAsync(request.RefreshToken);
            if(existing == null)
            {
                throw new ApplicationException("Invalid refresh token.");
            }

            // revoke old token
            await _repo.RevokeRefreshTokenASync(request.RefreshToken, DateTime.UtcNow);

            // get user entity
            var userEntity = await _repo.GetByIdAsync(existing.UserId);
            if(userEntity == null)
            {
                throw new ApplicationException("User not found.");
            }

            var userDto = MapToUserDto(userEntity);
            var accessToken = _tokenService.GenerateAccessToken(userDto);

            var newRefreshDto = _tokenService.GenerateRefreshToken(userEntity.UserId);
            var newRefreshEntity = MapRefreshDtoToEntity(newRefreshDto);
            await _repo.CreateRefreshTokenAsync(newRefreshEntity);

            return new AuthResponse
            {
                User = userDto,
                AccessToken = accessToken,
                RefreshToken = newRefreshDto.Token
            };
        }

        // Method logout
        public async Task LogoutAsync(LogoutRequest request, Guid userId)
        {
            if(request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if(string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return;
            }

            var existing = await _repo.GetRefreshTokenAsync(request.RefreshToken);
            if(existing == null)
            {
                return;
            }
            if(existing.UserId != userId)
            {
                return;
            }

            await _repo.RevokeRefreshTokenASync(request.RefreshToken, DateTime.UtcNow);
        }

        // Method forgot password
        public async Task ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var user = await _repo.GetByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogInformation("ForgotPassword requested for non-existent email: {Email}", request.Email);
                return;
            }

            var tokenBytes = RandomNumberGenerator.GetBytes(48);
            var token = Convert.ToBase64String(tokenBytes);
            var expiresAt = DateTime.UtcNow.AddHours(1);

            _passwordResetTokens[token] = (user.UserId, expiresAt);

            // Build front end reset link (escape token)
            var resetLink = $"https://your-frontend.example.com/reset-password?token={Uri.EscapeDataString(token)}";

            await _emailService.SendPasswordResetEmailAsync(user.Email, resetLink, token);

            _logger.LogInformation("Password reset token generated for user {UserId}", user.UserId);
        }

        // Method reset password
        public async Task ResetPasswordASync(ResetPasswordRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (!_passwordResetTokens.TryGetValue(request.Token, out var info))
            {
                throw new ApplicationException("Invalid or expired token.");
            }

            if (info.expiresAt <= DateTime.UtcNow)
            {
                _passwordResetTokens.TryRemove(request.Token, out _);
                throw new ApplicationException("Invalid or expired token.");
            }

            var user = await _repo.GetByIdAsync(info.userId);
            if (user == null)
            {
                throw new ApplicationException("User not found.");
            }

            var newHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _repo.UpdatePasswordAsync(user.UserId, newHash);

            _passwordResetTokens.TryRemove(request.Token, out _);
        }
        
        // HELPER
        private DOMAIN.Models.Users MapCreateUserDtoToEntity(CreateUserDto dto)
        {
            return new DOMAIN.Models.Users
            {
                UserId = dto.UserId,
                Email = dto.Email,
                PasswordHash = dto.PasswordHash,
                FullName = dto.FullName,
                Phone = dto.Phone,
                Locale = dto.Locale,
                IsActive = dto.IsActive ?? true,
                CreatedAt = DateTime.UtcNow
            };
        }

        private UserDto MapToUserDto(DOMAIN.Models.Users u)
        {
            return new UserDto
            {
                UserId = u.UserId,
                RoleId = u.RoleId,
                Email = u.Email,
                FullName = u.FullName,
                Phone = u.Phone,
                Locale = u.Locale,
                ProfilePhotoUrl = u.ProfilePhotoUrl,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            };
        }

        private Refreshtokens MapRefreshDtoToEntity(RefreshTokenDto dto)
        {
            return new Refreshtokens
            {
                TokenId = Guid.NewGuid(),
                UserId = dto.UserId,
                TokenHash = dto.Token,
                CreatedAt = dto.CreatedAt,
                ExpiresAt = dto.ExpiresAt,
                RevokedAt = null
            };
        }

    }
}
