using System;
using System.Collections.Generic;

namespace DOMAIN.Models;

public partial class Amenities
{
    public int AmenityId { get; set; }

    public string NameVi { get; set; } = null!;

    public string? NameEn { get; set; }

    public string? Slug { get; set; }

    public virtual ICollection<Propertyamenities> Propertyamenities { get; set; } = new List<Propertyamenities>();
}
