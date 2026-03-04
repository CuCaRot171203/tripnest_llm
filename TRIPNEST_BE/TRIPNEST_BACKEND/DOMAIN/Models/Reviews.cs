using System;
using System.Collections.Generic;

namespace DOMAIN.Models;

public partial class Reviews
{
    public Guid ReviewId { get; set; }

    public long PropertyId { get; set; }

    public Guid UserId { get; set; }

    public sbyte Rating { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Properties Property { get; set; } = null!;

    public virtual Users User { get; set; } = null!;
}
