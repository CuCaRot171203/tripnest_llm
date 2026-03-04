using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Property
{
    public class PropertyDto
    {
        public long PropertyId { get; set; }
        public Guid? OwnerUserId { get; set; }
        public Guid? CompanyId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
        public string? Country { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? AddressFormatted { get; set; }
        public string? PropertyType { get; set; }
        public decimal? PriceBase { get; set; }
        public string? Currency { get; set; }
        public bool? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<string>? PhotoUrls { get; set; } = new();
        public List<int>? Amenities { get; set; } = new();
    }
}
