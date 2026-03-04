using DOMAIN;
using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.Payment;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Repositories.Payment
{
    public class PaymentsRepository
    {
        private readonly TripnestDbContext _db;

        public PaymentsRepository(TripnestDbContext db)
        {
            _db = db;
        }

        public async Task<Payments> CreateAsync(Payments payment, CancellationToken ct = default)
        {
            payment.PaymentId = Guid.NewGuid();
            payment.CreatedAt = DateTime.UtcNow;
            await _db.Payments.AddAsync(payment, ct);
            await _db.SaveChangesAsync(ct);
            return payment;
        }

        public async Task<Payments?> GetByIdAsync(Guid paymentId, CancellationToken ct = default)
        {
            return await _db.Payments
                .Include(p => p.Booking)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId, ct);
        }

        public async Task<Payments?> GetByProviderRefAsync(string provider, string providerRef, CancellationToken ct = default)
        {
            return await _db.Payments
                .Include(p => p.Booking)
                .FirstOrDefaultAsync(p => p.Provider == provider && p.ProviderRef == providerRef, ct);
        }

        public async Task UpdateAsync(Payments payment, CancellationToken ct = default)
        {
            _db.Payments.Update(payment);
            await _db.SaveChangesAsync(ct);
        }
    }
}
