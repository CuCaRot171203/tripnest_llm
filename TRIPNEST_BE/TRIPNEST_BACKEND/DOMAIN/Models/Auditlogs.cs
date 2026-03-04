using System;
using System.Collections.Generic;

namespace DOMAIN.Models;

public partial class Auditlogs
{
    public Guid AuditId { get; set; }

    public string? EntityType { get; set; }

    public string? EntityId { get; set; }

    public string? Action { get; set; }

    public string? BeforeJson { get; set; }

    public string? AfterJson { get; set; }

    public Guid? PerformedBy { get; set; }

    public DateTime? PerformedAt { get; set; }

    public virtual Users? PerformedByNavigation { get; set; }
}
