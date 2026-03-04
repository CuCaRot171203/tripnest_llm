using DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Interfaces.Rooms
{
    public interface IRoomAvailabilityRepository
    {
        Task<Roomavailability?> GetByRoomAndDateAsync(long roomId, DateOnly date);
        Task AddAsync(Roomavailability item);
        Task UpdateAsync(Roomavailability item);
        Task<List<Roomavailability>> GetRangeAsync(long roomId, DateOnly from, DateOnly to);
    }
}
