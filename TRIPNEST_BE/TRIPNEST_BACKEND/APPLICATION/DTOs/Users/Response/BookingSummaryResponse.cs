using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Users.Response
{
    public class BookingSummaryResponse
    {
        public Guid BookingId { get; set; }
        public long PropertyId { get; set; }
        public string? PropertyName { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Status { get; set; }
    }
}
