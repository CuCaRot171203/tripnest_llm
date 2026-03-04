using System;
using System.Collections.Generic;

namespace DOMAIN.Models;

public partial class Properties
{
    public long PropertyId { get; set; }

    public Guid? OwnerUserId { get; set; }

    public Guid? CompanyId { get; set; }

    public string? TitleVi { get; set; }

    public string? TitleEn { get; set; }

    public string? DescriptionVi { get; set; }

    public string? DescriptionEn { get; set; }

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

    public virtual ICollection<Bookings> Bookings { get; set; } = new List<Bookings>();

    public virtual ICollection<Messages> Messages { get; set; } = new List<Messages>();

    public virtual ICollection<Propertyamenities> Propertyamenities { get; set; } = new List<Propertyamenities>();

    public virtual ICollection<Propertyphotos> Propertyphotos { get; set; } = new List<Propertyphotos>();

    public virtual ICollection<Reviews> Reviews { get; set; } = new List<Reviews>();

    public virtual ICollection<Rooms> Rooms { get; set; } = new List<Rooms>();
}
