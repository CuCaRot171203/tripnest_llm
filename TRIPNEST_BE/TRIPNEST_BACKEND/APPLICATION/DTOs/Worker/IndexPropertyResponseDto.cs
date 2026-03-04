using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Worker
{
    public class IndexPropertyResponseDto
    {
        public long PropertyId { get; set; }
        public bool Indexed { get; set; }
        public string? Message { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}
