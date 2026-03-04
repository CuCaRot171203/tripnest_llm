using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Booking.Response
{
    public class CancelBookingResponse
    {
        public Guid BookingId { get; set; }
        public string Status { get; set; } = "Cancelled";
        public bool RefundIssued { get; set; }
    }
}
