using APPLICATION.DTOs.Room.Request;
using APPLICATION.DTOs.Room.Response;
using APPLICATION.Interfaces.Rooms;
using DOMAIN;
using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Services.Rooms
{
    public class RoomsService : IRoomsService
    {
        private readonly IRoomsRepository _roomsRepo;
        private readonly IRoomAvailabilityRepository _availabilityRepo;
        private readonly IRoomPricesRepository _pricesRepo;
        private readonly TripnestDbContext _db;

        public RoomsService(
            IRoomsRepository roomsRepo,
            IRoomAvailabilityRepository availabilityRepo,
            IRoomPricesRepository pricesRepo,
            TripnestDbContext db)
        {
            _roomsRepo = roomsRepo;
            _availabilityRepo = availabilityRepo;
            _pricesRepo = pricesRepo;
            _db = db;
        }

        public async Task<RoomDto> CreateRoomAsync(CreateRoomRequest req)
        {
            var room = new DOMAIN.Models.Rooms
            {
                PropertyId = req.PropertyId,
                NameVi = req.NameVi,
                NameEn = req.NameEn,
                Capacity = req.Capacity,
                PricePerNight = req.PricePerNight,
                Stock = req.Stock,
                CancellationPolicy = req.CancellationPolicy,
                CreatedAt = DateTime.UtcNow
            };

            await _roomsRepo.AddAsync(room);
            await _roomsRepo.SaveChangesAsync();

            return MapToDto(room);
        }

        public async Task<RoomDto?> UpdateRoomAsync(long roomId, UpdateRoomRequest req)
        {
            var room = await _roomsRepo.GetByIdAsync(roomId);
            if (room == null) return null;

            if (req.NameVi is not null)
            {
                room.NameVi = req.NameVi;
            }
            if (req.NameEn is not null)
            {
                room.NameEn = req.NameEn;
            }
            if (req.Capacity is not null)
            {
                room.Capacity = req.Capacity;
            }
            if (req.PricePerNight is not null)
            {
                room.PricePerNight = req.PricePerNight;
            }
            if (req.Stock is not null)
            {
                room.Stock = req.Stock;
            }
            if (req.CancellationPolicy is not null)
            {
                room.CancellationPolicy = req.CancellationPolicy;
            }

            room.UpdatedAt = DateTime.UtcNow;

            await _roomsRepo.UpdateAsync(room);
            await _roomsRepo.SaveChangesAsync();

            return MapToDto(room);
        }

        public async Task<bool> DeleteRoomAsync(long roomId)
        {
            var room = await _roomsRepo.GetByIdAsync(roomId);
            if (room == null) return false;

            await _roomsRepo.SoftDeleteAsync(room);
            await _roomsRepo.SaveChangesAsync();
            return true;
        }

        public async Task<List<AvailabilityItemDto>> GetAvailabilityAsync(long roomId, DateOnly from, DateOnly to)
        {
            if (to < from)
            {
                throw new ArgumentException("to must be >= from");
            }

            var availList = await _availabilityRepo.GetRangeAsync(roomId, from, to);

            // If some dates missing, derive fallback from room.Stock or 0
            var dates = new List<AvailabilityItemDto>();
            var dayCount = (to.ToDateTime(TimeOnly.MinValue) - from.ToDateTime(TimeOnly.MinValue)).Days + 1;
            for (int i = 0; i < dayCount; i++)
            {
                var dt = from.AddDays(i);
                var a = availList.FirstOrDefault(x => x.Date == dt);
                dates.Add(new AvailabilityItemDto
                {
                    Date = dt,
                    AvailableCount = a?.AvailableCount ?? (await GetRoomStockFallbackAsync(roomId))
                });
            }

            return dates;
        }

        private async Task<int> GetRoomStockFallbackAsync(long roomId)
        {
            var room = await _roomsRepo.GetByIdAsync(roomId);
            return room?.Stock ?? 0;
        }

        public async Task<bool> BulkSetAvailabilityAsync(long roomId, BulkAvailabilityRequest req)
        {
            // Simple sanity checks
            var room = await _roomsRepo.GetByIdAsync(roomId);
            if (room == null)
            {
                return false;
            }

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in req.Dates)
                {
                    var existing = await _availabilityRepo.GetByRoomAndDateAsync(roomId, item.Date);
                    if (existing != null)
                    {
                        if (req.Override)
                        {
                            existing.AvailableCount = item.AvailableCount;
                            await _availabilityRepo.UpdateAsync(existing);
                        }
                        else
                        {
                            // upsert: replace count
                            existing.AvailableCount = item.AvailableCount;
                            await _availabilityRepo.UpdateAsync(existing);
                        }
                    }
                    else
                    {
                        var newItem = new Roomavailability
                        {
                            RoomId = roomId,
                            Date = item.Date,
                            AvailableCount = item.AvailableCount
                        };
                        await _availabilityRepo.AddAsync(newItem);
                    }
                }

                await _roomsRepo.SaveChangesAsync(); // Save through context
                await tx.CommitAsync();
                return true;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<PriceSlotDto> CreatePriceSlotAsync(long roomId, CreatePriceSlotRequest req)
        {
            var room = await _roomsRepo.GetByIdAsync(roomId);
            if (room == null)
            {
                throw new KeyNotFoundException("Room not found");
            }

            if (req.ValidTo < req.ValidFrom) throw new ArgumentException("ValidTo must be >= ValidFrom");

            var price = new Roomprices
            {
                RoomId = roomId,
                ValidFrom = req.ValidFrom,
                ValidTo = req.ValidTo,
                Price = req.Price,
                CreatedAt = DateTime.UtcNow
            };

            await _pricesRepo.AddAsync(price);
            await _roomsRepo.SaveChangesAsync();

            return new PriceSlotDto
            {
                RoomPriceId = price.RoomPriceId,
                RoomId = price.RoomId,
                ValidFrom = price.ValidFrom,
                ValidTo = price.ValidTo,
                Price = price.Price,
                CreatedAt = price.CreatedAt
            };
        }

        // HELPER
        private RoomDto MapToDto(DOMAIN.Models.Rooms r) => new()
        {
            RoomId = r.RoomId,
            PropertyId = r.PropertyId,
            NameVi = r.NameVi,
            NameEn = r.NameEn,
            Capacity = r.Capacity,
            PricePerNight = r.PricePerNight,
            Stock = r.Stock,
            CancellationPolicy = r.CancellationPolicy,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        };
    }
}
