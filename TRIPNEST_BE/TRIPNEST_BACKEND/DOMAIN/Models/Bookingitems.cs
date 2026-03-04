using System;
using System.Collections.Generic;

namespace DOMAIN.Models;

public partial class Bookingitems
{
    public long BookingItemId { get; set; }

    public Guid BookingId { get; set; }

    public long RoomId { get; set; }

    public decimal? Price { get; set; }

    public int? Nights { get; set; }

    public int? Qty { get; set; }

    public decimal? Subtotal { get; set; }

    public virtual Bookings Booking { get; set; } = null!;

    public virtual Rooms Room { get; set; } = null!;
}
