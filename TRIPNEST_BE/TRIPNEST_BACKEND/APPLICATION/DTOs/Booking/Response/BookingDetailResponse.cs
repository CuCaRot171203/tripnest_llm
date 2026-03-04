using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Booking.Response
{
    public class BookingDetailResponse
    {
        public Guid BookingId { get; set; }
        public long PropertyId { get; set; }
        public Guid UserId { get; set; }
        public string Status { get; set; } = null!;
        public DateOnly CheckinDate { get; set; }
        public DateOnly CheckoutDate { get; set; }
        public int GuestsCount { get; set; }
        public decimal TotalPrice { get; set; }
        public string Currency { get; set; } = null!;
        public List<BookingItemResponse> Items { get; set; } = new();
    }
}
