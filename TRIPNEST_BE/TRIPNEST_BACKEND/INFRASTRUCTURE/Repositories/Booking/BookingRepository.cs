using DOMAIN;
using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.Bookings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Repositories.Booking
{
    public class BookingRepository : IBookingRepository
    {
        private readonly TripnestDbContext _db;

        public BookingRepository(TripnestDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Bookings bookings)
        {
            await _db.Bookings.AddAsync(bookings);
            await _db.SaveChangesAsync();
        }

        public async Task<Bookings?> GetByIdAsync(Guid bookingId)
        {
            return await _db.Bookings
                .Include(b => b.Bookingitems)
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
        }

        public async Task UpdateAsync(Bookings booking)
        {
            _db.Bookings.Update(booking);
            await _db.SaveChangesAsync();
        }

        public async Task AddBookingItemsAsync(IEnumerable<Bookingitems> items)
        {
            await _db.Bookingitems.AddRangeAsync(items);
            await _db.SaveChangesAsync();
        }

        public async Task<int> DecrementRoomAvailabilityAsync(long roomId, DateOnly checkin, DateOnly checkout, int qty)
        {
            // Assuming a RoomAvailability table with columns: RoomId, Date, Available
            // We lock rows for the date range and decrement if enough availability exists.
            var start = checkin;
            var end = checkout.AddDays(-1);
            // Use transaction in service; here just raw SQL snippet for each date (inefficient but explicit)
            int totalAffected = 0;
            for (var d = start; d <= end; d = d.AddDays(1))
            {
                var sql = @"
                    UPDATE RoomAvailability
                    SET Available = Available - @p3
                    FROM RoomAvailability WITH (UPDLOCK, ROWLOCK)
                    WHERE RoomId = @p0 AND [Date] = @p1 AND Available >= @p3";
                var affected = await _db.Database.ExecuteSqlRawAsync(sql, roomId, d.ToDateTime(TimeOnly.MinValue), qty);
                totalAffected += affected;
            }
            return totalAffected;
        }

        public async Task<int> IncrementRoomAvailabilityAsync(long roomId, DateOnly checkin, DateOnly checkout, int qty)
        {
            var start = checkin;
            var end = checkout.AddDays(-1);
            int totalAffected = 0;
            for (var d = start; d <= end; d = d.AddDays(1))
            {
                var sql = @"
                    UPDATE RoomAvailability
                    SET Available = Available + @p3
                    FROM RoomAvailability WITH (UPDLOCK, ROWLOCK)
                    WHERE RoomId = @p0 AND [Date] = @p1";
                var affected = await _db.Database.ExecuteSqlRawAsync(sql, roomId, d.ToDateTime(TimeOnly.MinValue), qty);
                totalAffected += affected;
            }
            return totalAffected;
        }

        public async Task<IDisposable> BeginTransactionAsync()
        {
            var tran = await _db.Database.BeginTransactionAsync();
            return tran;
        }
    }
}
