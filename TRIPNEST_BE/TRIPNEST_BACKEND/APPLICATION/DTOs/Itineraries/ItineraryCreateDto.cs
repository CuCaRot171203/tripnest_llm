using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Itineraries
{
    public class ItineraryCreateDto
    {
        public string? NameVi { get; set; }
        public string? NameEn { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? Metadata { get; set; }
        public List<WaypointDto>? Waypoints { get; set; }
    }
}
