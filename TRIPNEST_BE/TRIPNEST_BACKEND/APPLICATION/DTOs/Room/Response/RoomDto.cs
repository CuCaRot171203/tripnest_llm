using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Room.Response
{
    public class RoomDto
    {
        public long RoomId { get; set; }
        public long PropertyId { get; set; }
        public string? NameVi { get; set; }
        public string? NameEn { get; set; }
        public int? Capacity { get; set; }
        public decimal? PricePerNight { get; set; }
        public int? Stock { get; set; }
        public string? CancellationPolicy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
