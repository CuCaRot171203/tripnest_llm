using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Itineraries
{
    public class SuggestItineraryResponseDto
    {
        public string ItineraryText { get; set; } = default!;
        public List<ItineraryResponseDto>? StructuredItineraries { get; set; }
    }
}
