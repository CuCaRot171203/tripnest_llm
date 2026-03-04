using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Room.Request
{
    public class PriceSlotDto
    {
        public long RoomPriceId { get; set; }
        public long RoomId { get; set; }
        public DateOnly ValidFrom { get; set; }
        public DateOnly ValidTo { get; set; }
        public decimal Price { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
