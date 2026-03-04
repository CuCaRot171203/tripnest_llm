using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Booking.Request
{
    public class ModifyBookingItemRequest
    {
        public int? NewQty { get; set; }
        public int? NewNights { get; set; }
    }
}
