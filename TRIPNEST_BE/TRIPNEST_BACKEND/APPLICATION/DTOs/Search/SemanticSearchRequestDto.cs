using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.DTOs.Search
{
    public class SemanticSearchRequestDto
    {
        public string Text { get; set; } = null!;
        public Dictionary<string, string>? Filters { get; set; } 
        public int? TopK { get; set; } = 10;
    }
}
