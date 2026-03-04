using System;
using System.Collections.Generic;

namespace DOMAIN.Models;

public partial class Rooms
{
    public long RoomId { get; set; }

    public long PropertyId { get; set; }

    public string? NameVi { get; set; }

    public string? NameEn { get; set; }

    public int? Capacity { get; set; }

    public decimal? PricePerNight { get; set; }

    public int? Stock { get; set; }

    public string? CancellationPolicy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Bookingitems> Bookingitems { get; set; } = new List<Bookingitems>();

    public virtual Properties Property { get; set; } = null!;

    public virtual ICollection<Roomavailability> Roomavailability { get; set; } = new List<Roomavailability>();

    public virtual ICollection<Roomprices> Roomprices { get; set; } = new List<Roomprices>();
}
