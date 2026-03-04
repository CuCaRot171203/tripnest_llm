using DOMAIN;
using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.Worker;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Repositories.Worker
{
    public class BookingsRepository : IBookingsRepository
    {
        private readonly TripnestDbContext _db;

        public BookingsRepository( TripnestDbContext db)
        {
            _db = db;
        }

        public async Task<List<Bookings>> GetUpcomingBookingsForRemindersAsync(
            DateTime forDateFromUtc, 
            DateTime forDateToUtc, 
            CancellationToken ct = default)
        {
            return await _db.Bookings
                .Include(b => b.User)
                .Include(b => b.Property)
                .Where(b => b.Status == "confirmed"
                            && b.CheckinDate.ToDateTime(TimeOnly.MinValue) >= forDateFromUtc.Date
                            && b.CheckinDate.ToDateTime(TimeOnly.MinValue) <= forDateToUtc.Date)
                .AsNoTracking()
                .ToListAsync(ct);
        }
    }
}
