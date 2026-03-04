using System;
using System.Collections.Generic;

namespace DOMAIN.Models;

public partial class Messages
{
    public Guid MessageId { get; set; }

    public Guid? FromUserId { get; set; }

    public Guid? ToUserId { get; set; }

    public long? PropertyId { get; set; }

    public string? Content { get; set; }

    public bool? IsAi { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Users? FromUser { get; set; }

    public virtual Properties? Property { get; set; }

    public virtual Users? ToUser { get; set; }
}
