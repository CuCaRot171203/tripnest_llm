using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Search
{
    public class SuggestionDto
    {
        public string Text { get; set; } = null!;
        public string? Type { get; set; } 
        public long? PropertyId { get; set; } 
    }
}
