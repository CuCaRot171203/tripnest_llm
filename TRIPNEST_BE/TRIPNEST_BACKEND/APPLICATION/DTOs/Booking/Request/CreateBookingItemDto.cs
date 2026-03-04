using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Booking.Request
{
    public class CreateBookingItemDto
    {
        public long RoomId { get; set; }
        public int Qty { get; set; }
        public int Nights { get; set; }
    }
}
