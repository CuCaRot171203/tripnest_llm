using System;
using System.Collections.Generic;

namespace DOMAIN.Models;

public partial class Notifications
{
    public Guid NotificationId { get; set; }

    public Guid UserId { get; set; }

    public string? Type { get; set; }

    public string? Payload { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Users User { get; set; } = null!;
}
