using APPLICATION.DTOs.Auth.Response;
using APPLICATION.Interfaces.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Services.Auth
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly byte[] _key;

        public TokenService(IConfiguration config)
        {
            _config = config;
            var secret = _config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key missing");
            _key = Encoding.UTF8.GetBytes(secret);
        }

        public string GenerateAccessToken(UserDto user, TimeSpan? lifetime = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(_key);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim("role", (user.RoleId ?? 0).ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(lifetime ?? TimeSpan.FromMinutes(60)),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public RefreshTokenDto GenerateRefreshToken(Guid userId, TimeSpan? lifetime = null)
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var token = Convert.ToBase64String(tokenBytes);

            var createdAt = DateTime.UtcNow;
            var expiresAt = createdAt.Add(lifetime ?? TimeSpan.FromDays(14));

            return new RefreshTokenDto
            {
                UserId = userId,
                Token = token,
                CreatedAt = createdAt,
                ExpiresAt = expiresAt
            };
        }
    }
}
