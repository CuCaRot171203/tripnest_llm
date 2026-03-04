using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Search
{
    public class SearchResultItemDto
    {
        public long PropertyId { get; set; }
        public string? Title { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public decimal? PriceBase { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? ThumbnailUrl { get; set; }
        public double? DistanceKm { get; set; } 
        public int? MinCapacity { get; set; }
    }
}
