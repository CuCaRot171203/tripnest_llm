using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Room.Request
{
    public class UpdateRoomRequest
    {
        [MaxLength(200)]
        public string? NameVi { get; set; }

        [MaxLength(200)]
        public string? NameEn { get; set; }

        [Range(1, 100)]
        public int? Capacity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? PricePerNight { get; set; }

        [Range(0, int.MaxValue)]
        public int? Stock { get; set; }

        public string? CancellationPolicy { get; set; }
    }
}
