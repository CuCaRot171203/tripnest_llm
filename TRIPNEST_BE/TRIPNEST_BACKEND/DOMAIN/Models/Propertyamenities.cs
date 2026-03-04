using System;
using System.Collections.Generic;

namespace DOMAIN.Models;

public partial class Propertyamenities
{
    public long Id { get; set; }

    public long PropertyId { get; set; }

    public int AmenityId { get; set; }

    public virtual Amenities Amenity { get; set; } = null!;

    public virtual Properties Property { get; set; } = null!;
}
