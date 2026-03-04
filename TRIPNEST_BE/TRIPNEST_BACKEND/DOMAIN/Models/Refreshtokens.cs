using System;
using System.Collections.Generic;

namespace DOMAIN.Models;

public partial class Refreshtokens
{
    public Guid TokenId { get; set; }

    public Guid UserId { get; set; }

    public string TokenHash { get; set; } = null!;

    public string? DeviceInfo { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public virtual Users User { get; set; } = null!;
}
