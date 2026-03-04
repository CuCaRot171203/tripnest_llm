using APPLICATION.DTOs.Room.Request;
using APPLICATION.DTOs.Room.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Interfaces.Rooms
{
    public interface IRoomsService
    {
        Task<RoomDto> CreateRoomAsync(CreateRoomRequest req);
        Task<RoomDto?> UpdateRoomAsync(long roomId, UpdateRoomRequest req);
        Task<bool> DeleteRoomAsync(long roomId);
        Task<List<AvailabilityItemDto>> GetAvailabilityAsync(long roomId, DateOnly from, DateOnly to);
        Task<bool> BulkSetAvailabilityAsync(long roomId, BulkAvailabilityRequest req);
        Task<PriceSlotDto> CreatePriceSlotAsync(long roomId, CreatePriceSlotRequest req);
    }
}
