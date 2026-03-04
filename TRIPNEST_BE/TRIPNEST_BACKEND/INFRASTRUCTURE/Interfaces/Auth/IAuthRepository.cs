using DOMAIN.Models;

namespace INFRASTRUCTURE.Interfaces.Auth
{
    public interface IAuthRepository
    {
        // Read/ write users
        Task<DOMAIN.Models.Users?> GetByEmailAsync(string email);
        Task<DOMAIN.Models.Users?> GetByIdAsync(Guid userId);
        Task CreateUserAsync(DOMAIN.Models.Users users);
        Task UpdateUserAsync(DOMAIN.Models.Users users);

        // Password update
        Task UpdatePasswordAsync(Guid userId, string newPasswordHash);

        // refresh token
        Task CreateRefreshTokenAsync(Refreshtokens refreshtokens);
        Task<Refreshtokens?> GetRefreshTokenAsync(string token);
        Task RevokeRefreshTokenASync(string token, DateTime revokedAt);

        Task<bool> VerifyCredentialsAsync(string email, string password);
    }
}
