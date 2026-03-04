using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Interfaces.Rooms
{
    public interface IRoomsRepository
    {
        Task<DOMAIN.Models.Rooms?> GetByIdAsync(long roomId);
        Task AddAsync(DOMAIN.Models.Rooms room);
        Task UpdateAsync(DOMAIN.Models.Rooms room);
        Task SoftDeleteAsync(DOMAIN.Models.Rooms room);
        Task SaveChangesAsync();
    }
}
