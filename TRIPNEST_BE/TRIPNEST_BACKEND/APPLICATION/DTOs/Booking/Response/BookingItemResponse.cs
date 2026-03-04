using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Booking.Response
{
    public class BookingItemResponse
    {
        public long BookingItemId { get; set; }
        public long RoomId { get; set; }
        public int Qty { get; set; }
        public int Nights { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal { get; set; }
    }
}
