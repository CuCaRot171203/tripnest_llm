using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Interfaces.Webhook
{
    public interface IBookingsRepository
    {
        Task<DOMAIN.Models.Bookings?> GetByIdAsync(Guid bookingId, CancellationToken ct = default);
        Task<DOMAIN.Models.Bookings?> GetByPropertyAndDatesAsync(long propertyId, DateOnly checkin, DateOnly checkout, CancellationToken ct = default);
        Task AddAsync(DOMAIN.Models.Bookings booking, CancellationToken ct = default);
        Task UpdateAsync(DOMAIN.Models.Bookings booking, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
