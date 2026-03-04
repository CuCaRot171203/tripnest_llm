using DOMAIN;
using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Repositories.User
{
    public class UserRepository : IUsersRepository
    {
        private readonly TripnestDbContext _db;

        public UserRepository(TripnestDbContext db) 
        { 
            _db = db; 
        }

        // Get user by id
        public async Task<Users?> GetByIdAsync(Guid id)
        {
            return await _db.Users
                .Include(u => u.Role)
                .Include(u => u.Companyemployees)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == id);
        }

        // Get user by email
        public async Task<Users?> GetByEmailAsync(string email)
        {
            if(string.IsNullOrWhiteSpace(email))
            {
                return null;
            }
            return await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        // Update profile
        public async Task UpdateProfileAsync(Users users)
        {
            var entity = await _db.Users.FindAsync(users.UserId);
            if(entity == null)
            {
                throw new KeyNotFoundException("User not found");
            }
            
            entity.FullName = users.FullName;
            entity.Phone = users.Phone;
            entity.Locale = users.Locale;
            entity.ProfilePhotoUrl = users.ProfilePhotoUrl;
            entity.UpdatedAt = DateTime.UtcNow;

            _db.Users.Update(entity);
            await _db.SaveChangesAsync();
        }

        // create user when not verify (invite flow)
        public async Task CreateUnverifiedUserAsync(string email, Guid userId)
        {
            var exist = await _db.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
            if (exist)
            {
                return;
            }

            var entity = new Users
            {
                UserId = userId,
                Email = email,
                IsActive = false,
                CreatedAt = DateTime.UtcNow,
            };

            _db.Users.Add(entity);
            await _db.SaveChangesAsync();
        }

        // add company employee
        public async Task AddCompanyEmployeeAsync(Guid companyId, Guid userId, string role)
        {
            var ce = new Companyemployees
            {
                CompanyEmployeeId = Guid.NewGuid(),
                CompanyId = companyId,
                UserId = userId,
                Title = role,
                IsActive = true,
                JoinedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            _db.Companyemployees.Add(ce);
            await _db.SaveChangesAsync();
        }

        // Lấy booking summary cho user
        public async Task<IEnumerable<Bookings>> GetBookingsForUserAsync(Guid userId)
        {
            return await _db.Bookings
                .AsNoTracking()
                .Include(b => b.Property)
                .Where(b => b.UserId == userId)
                .ToListAsync();
        }
    }
}
