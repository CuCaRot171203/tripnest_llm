using System;
using System.Collections.Generic;

namespace DOMAIN.Models;

public partial class Itineraries
{
    public Guid ItineraryId { get; set; }

    public Guid UserId { get; set; }

    public string? NameVi { get; set; }

    public string? NameEn { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? Metadata { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Users User { get; set; } = null!;

    public virtual ICollection<Waypoints> Waypoints { get; set; } = new List<Waypoints>();
}
