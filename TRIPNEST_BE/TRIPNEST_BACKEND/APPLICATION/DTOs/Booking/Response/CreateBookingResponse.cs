using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Booking.Response
{
    public class CreateBookingResponse
    {
        public Guid BookingId { get; set; }
        public string Status { get; set; } = "Pending";
        public string PaymentSession { get; set; } = null!;
    }
}
