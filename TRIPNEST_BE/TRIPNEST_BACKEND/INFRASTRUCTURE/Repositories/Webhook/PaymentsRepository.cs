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
    public class PaymentsRepository : IPaymentsRepository
    {
        private readonly TripnestDbContext _db;

        public PaymentsRepository(TripnestDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(
            Payments payment, 
            CancellationToken ct = default)
        {
            await _db.Payments.AddAsync(payment, ct);
        }

        public async Task<Payments?> GetByIdAsync(
            Guid paymentId, 
            CancellationToken ct = default)
        {
            return await _db.Payments
                .Include(p => p.Booking)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId, ct);
        }

        public async Task<Payments?> GetByProviderRefAsync(
            string provider, 
            string providerRef, 
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(providerRef))
            {
                return null;
            }
            return await _db.Payments
                .Include(p => p.Booking)
                .FirstOrDefaultAsync(
                p => p.ProviderRef == providerRef 
                && p.Provider == provider, ct);
        }

        public async Task<IEnumerable<Payments>> GetByBookingIdAsync(Guid bookingId, CancellationToken ct = default)
        {
            return await _db.Payments
                .Where(p => p.BookingId == bookingId)
                .ToListAsync(ct);
        }

        public Task UpdateAsync(Payments payment, CancellationToken ct = default)
        {
            _db.Payments.Update(payment);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken ct = default)
        {
            return _db.SaveChangesAsync(ct);
        }
    }
}
