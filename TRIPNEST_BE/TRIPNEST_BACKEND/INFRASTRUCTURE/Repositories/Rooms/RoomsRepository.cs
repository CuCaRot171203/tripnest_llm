using DOMAIN;
using INFRASTRUCTURE.Interfaces.Rooms;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Repositories.Rooms
{
    public class RoomsRepository : IRoomsRepository
    {
        private readonly TripnestDbContext _db;

        public RoomsRepository(TripnestDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(DOMAIN.Models.Rooms room)
        {
            room.CreatedAt = DateTime.UtcNow;
            await _db.Rooms.AddAsync(room);
        }

        public async Task<DOMAIN.Models.Rooms?> GetByIdAsync(long roomId)
        {
            return await _db.Rooms
                .Include(r => r.Roomavailability)
                .Include(r => r.Roomprices)
                .FirstOrDefaultAsync(r => r.RoomId == roomId);
        }

        public Task UpdateAsync(DOMAIN.Models.Rooms room)
        {
            room.UpdatedAt = DateTime.UtcNow;
            _db.Rooms.Update(room);
            return Task.CompletedTask;
        }

        public async Task SoftDeleteAsync(DOMAIN.Models.Rooms room)
        {
            room.UpdatedAt = DateTime.UtcNow;
            _db.Rooms.Update(room);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
