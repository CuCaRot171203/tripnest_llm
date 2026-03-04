using System;
using System.Collections.Generic;

namespace DOMAIN.Models;

public partial class Propertyphotos
{
    public long PhotoId { get; set; }

    public long PropertyId { get; set; }

    public string Url { get; set; } = null!;

    public int? Order { get; set; }

    public string? Meta { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Properties Property { get; set; } = null!;
}
