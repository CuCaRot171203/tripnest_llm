using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Room.Request
{
    public class BulkAvailabilityRequest
    {
        [Required]
        public List<AvailabilityItemDto> Dates { get; set; } = new();
       public bool Override { get; set; } = false;
    }
}
