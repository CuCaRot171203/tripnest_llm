using DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Interfaces.Bookings
{
    public interface IBookingRepository
    {
        Task<DOMAIN.Models.Bookings?> GetByIdAsync(Guid bookingId);
        Task AddAsync(DOMAIN.Models.Bookings booking);
        Task UpdateAsync(DOMAIN.Models.Bookings booking);

        // Booking items
        Task AddBookingItemsAsync(IEnumerable<Bookingitems> items);

        // Room availability ops should be transactional and lock rows
        Task<int> DecrementRoomAvailabilityAsync(long roomId, DateOnly checkin, DateOnly checkout, int qty); 
        Task<int> IncrementRoomAvailabilityAsync(long roomId, DateOnly checkin, DateOnly checkout, int qty);

        // Use this to start/commit/rollback transactions if needed:
        Task<IDisposable> BeginTransactionAsync(); 
    }
}
