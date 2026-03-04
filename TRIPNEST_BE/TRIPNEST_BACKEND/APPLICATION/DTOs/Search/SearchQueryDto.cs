using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Search
{
    public class SearchQueryDto
    {
        public string? Q { get; set; }
        public string? City { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
        public double? Radius { get; set; } 
        public DateTime? Checkin { get; set; }
        public DateTime? Checkout { get; set; }
        public List<int>? Amenities { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Sort { get; set; } 
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
