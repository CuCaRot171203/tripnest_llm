using DOMAIN;
using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.Auth;
using Microsoft.EntityFrameworkCore;

namespace INFRASTRUCTURE.Repositories.Auth
{
    public class AuthRepository : IAuthRepository
    {
        private readonly TripnestDbContext _db;

        public AuthRepository(TripnestDbContext db)
        {
            _db = db;
        }

        // Method to create user
        public async Task CreateUserAsync(Users user)
        {
            if(user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.CreatedAt = DateTime.UtcNow;
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }
    
        // Method get by email 
        public async Task<Users?> GetByEmailAsync(string email)
        {
            return await _db.Users.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        // Method get by Id
        public async Task<Users?> GetByIdAsync(Guid userId)
        {
            return await _db.Users
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        // Method to update user
        public async Task UpdateUserAsync(Users users)
        {
            if(users == null)
            {
                throw new ArgumentNullException(nameof(users));
            }
            var existing = await _db.Users
                .FirstOrDefaultAsync(u => u.UserId == users.UserId);
            if (existing == null)
            {
                return;
            }

            // Update allowed fields
            existing.FullName = users.FullName;
            existing.Phone = users.Phone;
            existing.Locale = users.Locale;
            existing.ProfilePhotoUrl = users.ProfilePhotoUrl;
            existing.IsActive = users.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;

            _db.Users.Update(existing);
            await _db.SaveChangesAsync();
        }

        // Method to update password
        public async Task UpdatePasswordAsync(Guid userId, string newPasswordHash)
        {
            var existing = await _db.Users
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (existing == null)
            {
                return;
            }

            existing.PasswordHash = newPasswordHash;
            existing.UpdatedAt = DateTime.UtcNow;

            _db.Users.Update(existing);
            await _db.SaveChangesAsync();
        }

        // Method create refresh token
        public async Task CreateRefreshTokenAsync(Refreshtokens refreshToken)
        {
            if(refreshToken == null)
            {
                throw new ArgumentNullException(nameof(refreshToken));
            }
            refreshToken.CreatedAt = refreshToken.CreatedAt ?? DateTime.UtcNow;
            _db.Refreshtokens.Add(refreshToken);
            await _db.SaveChangesAsync();
        }

        // Method get refresh token
        public async Task<Refreshtokens?> GetRefreshTokenAsync(string token)
        {
            return await _db.Refreshtokens
                .Include(x => x.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.TokenHash == token);
        }

        // Method revoke refresh token
        public async Task RevokeRefreshTokenASync(string token, DateTime revokedAt)
        {
            var r = await _db.Refreshtokens
                .FirstOrDefaultAsync(x => x.TokenHash == token);
            if(r == null)
            {
                return;
            }

            r.RevokedAt = revokedAt;
            _db.Refreshtokens.Update(r);
            await _db.SaveChangesAsync();
        }

        // Method verify credential
        public async Task<bool> VerifyCredentialsAsync(string email, string password)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(x => x.Email == email);
            if(user == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                return false ;
            }

            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }
    }
}
