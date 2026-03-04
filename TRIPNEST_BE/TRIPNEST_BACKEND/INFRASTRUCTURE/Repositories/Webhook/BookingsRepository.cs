using DOMAIN;
using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.Webhook;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Repositories.Webhook
{
    public class BookingsRepository : IBookingsRepository
    {
        private readonly TripnestDbContext _db;

        public BookingsRepository(TripnestDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Bookings booking, CancellationToken ct = default)
        {
            await _db.Bookings.AddAsync(booking, ct);
        }

        public async Task<Bookings?> GetByIdAsync(Guid bookingId, CancellationToken ct = default)
        {
            return await _db.Bookings
                .Include(b => b.Payments)
                .Include(b => b.Bookingitems)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId, ct);
        }

        public async Task<Bookings?> GetByPropertyAndDatesAsync(long propertyId, DateOnly checkin, DateOnly checkout, CancellationToken ct = default)
        {
            return await _db.Bookings
                .Where(b => b.PropertyId == propertyId && b.CheckinDate == checkin && b.CheckoutDate == checkout)
                .FirstOrDefaultAsync(ct);
        }

        public Task UpdateAsync(Bookings booking, CancellationToken ct = default)
        {
            _db.Bookings.Update(booking);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken ct = default)
        {
            return _db.SaveChangesAsync(ct);
        }
    }
}
