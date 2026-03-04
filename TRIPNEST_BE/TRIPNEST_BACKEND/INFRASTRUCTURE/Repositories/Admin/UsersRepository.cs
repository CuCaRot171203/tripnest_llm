using DOMAIN;
using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.Admin;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Repositories.Admin
{
    public class UsersRepository : IUsersRepository
    {
        private readonly TripnestDbContext _db;

        public UsersRepository(TripnestDbContext db)
        {
            _db = db;
        }

        public async Task<Users?> GetByIdAsync(Guid userId, CancellationToken ct = default)
        {
            return await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == userId, ct);
        }

        public async Task<(IEnumerable<Users> Items, int Total)> GetUsersAsync(
            int? roleId, bool? isActive, int page = 1, 
            int pageSize = 50, CancellationToken ct = default)
        {
            if (page <= 0)
            {
                page = 1;
            }
            if (pageSize <= 0)
            {
                pageSize = 50;
            }

            var q = _db.Users.AsQueryable();

            if (roleId.HasValue)
            {
                q = q.Where(u => u.RoleId == roleId.Value);
            }

            if (isActive.HasValue)
            {
                q = q.Where(u => u.IsActive == isActive.Value);
            }

            var total = await q.CountAsync(ct);

            var items = await q
                .OrderBy(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync(ct);

            return (items, total);
        }

        public Task UpdateAsync(Users entity, CancellationToken ct = default)
        {
            _db.Users.Update(entity);
            return Task.CompletedTask;
        }

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return _db.SaveChangesAsync(ct);
        }
    }
}
