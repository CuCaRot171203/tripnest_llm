using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Booking.Request
{
    public class CreateBookingRequest
    {
        public long PropertyId { get; set; }
        public List<CreateBookingItemDto> Items { get; set; } = new();
        public DateOnly CheckinDate { get; set; }
        public DateOnly CheckoutDate { get; set; }
        public int GuestsCount { get; set; }
        public string PaymentMethod { get; set; } = "online";
        public string Currency { get; set; } = "USD";
    }
}
