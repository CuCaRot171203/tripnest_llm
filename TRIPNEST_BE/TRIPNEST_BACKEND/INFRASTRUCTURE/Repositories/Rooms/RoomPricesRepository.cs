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
    public class RoomPricesRepository : IRoomPricesRepository
    {
        private readonly TripnestDbContext _db;

        public RoomPricesRepository(TripnestDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Roomprices price)
        {
            price.CreatedAt = DateTime.UtcNow;
            await _db.Roomprices.AddAsync(price);
        }

        public async Task<List<Roomprices>> GetPricesForRoomAsync(long roomId, DateOnly from, DateOnly to)
        {
            return await _db.Roomprices
                .Where(p => p.RoomId == roomId 
                    && !(p.ValidTo < from || p.ValidFrom > to))
                .ToListAsync();
        }
    }
}
