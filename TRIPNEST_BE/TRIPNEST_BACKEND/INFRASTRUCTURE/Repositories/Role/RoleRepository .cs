using APPLICATION.Interfaces.Roles;
using DOMAIN;
using DOMAIN.Models;
using Microsoft.EntityFrameworkCore;

namespace INFRASTRUCTURE.Repositories.Role
{
    public class RoleRepository : IRoleRepository
    {
        private readonly TripnestDbContext _db;

        public RoleRepository(TripnestDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Roles>> GetAllAsync()
        {
            return await _db.Set<Roles>()
                .AsNoTracking().ToListAsync();
        }

        public async Task<Roles?> GetByNameAsync(string name)
        {
            return await _db.Set<Roles>()
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Name == name);
        }

        public async Task<Roles> AddAsync(Roles role)
        {
            role.CreatedAt = DateTime.UtcNow;
            var entry = await _db.Set<Roles>().AddAsync(role);
            await _db.SaveChangesAsync();
            return entry.Entity;
        }
    }
}
