using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Itineraries
{
    public class EmbeddingRequestDto
    {
        public string SourceId { get; set; } = default!;
        public string Payload { get; set; } = default!;
    }

}
