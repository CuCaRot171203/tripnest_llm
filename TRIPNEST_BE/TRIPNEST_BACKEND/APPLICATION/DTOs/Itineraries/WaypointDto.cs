using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Itineraries
{
    public class WaypointDto
    {
        public long WaypointId { get; set; }
        public Guid ItineraryId { get; set; }
        public int? Order { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
        public string? PlaceId { get; set; }
        public DateTime? Arrival { get; set; }
    }
}
