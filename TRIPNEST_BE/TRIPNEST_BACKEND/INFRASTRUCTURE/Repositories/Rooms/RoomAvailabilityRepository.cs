using DOMAIN;
using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.Rooms;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Repositories.Rooms
{
    public class RoomAvailabilityRepository : IRoomAvailabilityRepository
    {
        private readonly TripnestDbContext _db;

        public RoomAvailabilityRepository(TripnestDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Roomavailability item)
        {
            await _db.Roomavailability
                .AddAsync(item);
        }

        public async Task<Roomavailability?> GetByRoomAndDateAsync(long roomId, DateOnly date)
        {
            return await _db.Roomavailability
                .FirstOrDefaultAsync(a => a.RoomId == roomId && a.Date == date);
        }

        public async Task UpdateAsync(Roomavailability item)
        {
            _db.Roomavailability.Update(item);
            await Task.CompletedTask;
        }

        public async Task<List<Roomavailability>> GetRangeAsync(long roomId, DateOnly from, DateOnly to)
        {
            return await _db.Roomavailability
                .Where(a => a.RoomId == roomId 
                    && a.Date >= from && a.Date <= to)
                .ToListAsync();
        }
    }
}
