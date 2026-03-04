using DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Interfaces.Rooms
{
    public interface IRoomPricesRepository
    {
        Task AddAsync(Roomprices price);
        Task<List<Roomprices>> GetPricesForRoomAsync(long roomId, DateOnly from, DateOnly to);
    }
}
