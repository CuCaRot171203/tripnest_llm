using System;
using System.Collections.Generic;

namespace DOMAIN.Models;

public partial class Bookings
{
    public Guid BookingId { get; set; }

    public Guid UserId { get; set; }

    public long PropertyId { get; set; }

    public string? Status { get; set; }

    public DateOnly CheckinDate { get; set; }

    public DateOnly CheckoutDate { get; set; }

    public int? GuestsCount { get; set; }

    public decimal? TotalPrice { get; set; }

    public string? Currency { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? Version { get; set; }

    public virtual ICollection<Bookingitems> Bookingitems { get; set; } = new List<Bookingitems>();

    public virtual ICollection<Payments> Payments { get; set; } = new List<Payments>();

    public virtual Properties Property { get; set; } = null!;

    public virtual Users User { get; set; } = null!;
}
