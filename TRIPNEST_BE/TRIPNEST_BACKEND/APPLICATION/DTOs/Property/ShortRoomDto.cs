using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Property
{
    public class ShortRoomDto
    {
        public long RoomId { get; set; }
        public string? Name { get; set; }
        public decimal? Price { get; set; }
    }
}
