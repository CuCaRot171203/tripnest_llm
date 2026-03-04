using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Itineraries
{
    public class SuggestItineraryRequestDto
    {
        public string Location { get; set; } = default!;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public List<string>? Interests { get; set; }
        public decimal? Budget { get; set; }
    }
}
