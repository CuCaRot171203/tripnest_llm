using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Room.Request
{
    public class AvailabilityItemDto
    {
        [Required]
        public DateOnly Date { get; set; }

        [Range(0, int.MaxValue)]
        public int AvailableCount { get; set; }
    }
}
