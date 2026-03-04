using APPLICATION.DTOs.Booking.Request;
using APPLICATION.DTOs.Booking.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Interfaces.Booking
{
    public interface IBookingService
    {
        Task<CreateBookingResponse> CreateBookingAsync(Guid userId, CreateBookingRequest dto);
        Task<BookingDetailResponse?> GetBookingAsync(Guid bookingId, Guid callerUserId, bool isAdminOrHost);
        Task<CancelBookingResponse> CancelBookingAsync(Guid bookingId, Guid callerUserId, bool isAdminOrHost);
        Task<BookingDetailResponse?> ModifyBookingItemAsync(Guid bookingId, long bookingItemId, ModifyBookingItemRequest dto, Guid callerUserId, bool isAdminOrHost);
    }
}
