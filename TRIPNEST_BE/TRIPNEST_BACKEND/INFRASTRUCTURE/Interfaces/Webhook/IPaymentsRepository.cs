using DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Interfaces.Webhook
{
    public interface IPaymentsRepository
    {
        Task<Payments?> GetByIdAsync(Guid paymentId, CancellationToken ct = default);
        Task<Payments?> GetByProviderRefAsync(string provider, string providerRef, CancellationToken ct = default);
        Task<IEnumerable<Payments>> GetByBookingIdAsync(Guid bookingId, CancellationToken ct = default);
        Task AddAsync(Payments payment, CancellationToken ct = default);
        Task UpdateAsync(Payments payment, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
