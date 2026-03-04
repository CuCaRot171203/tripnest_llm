using System;
using System.Collections.Generic;

namespace DOMAIN.Models;

public partial class Roomavailability
{
    public long AvailabilityId { get; set; }

    public long RoomId { get; set; }

    public DateOnly Date { get; set; }

    public int AvailableCount { get; set; }

    public virtual Rooms Room { get; set; } = null!;
}
