using System;
using System.Collections.Generic;

namespace DOMAIN.Models;

public partial class Waypoints
{
    public long WaypointId { get; set; }

    public Guid ItineraryId { get; set; }

    public int? Order { get; set; }

    public double? Lat { get; set; }

    public double? Lng { get; set; }

    public string? PlaceId { get; set; }

    public DateTime? Arrival { get; set; }

    public virtual Itineraries Itinerary { get; set; } = null!;
}
