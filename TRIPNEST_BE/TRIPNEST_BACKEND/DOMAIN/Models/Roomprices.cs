using System;
using System.Collections.Generic;

namespace DOMAIN.Models;

public partial class Roomprices
{
    public long RoomPriceId { get; set; }

    public long RoomId { get; set; }

    public DateOnly ValidFrom { get; set; }

    public DateOnly ValidTo { get; set; }

    public decimal Price { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Rooms Room { get; set; } = null!;
}
