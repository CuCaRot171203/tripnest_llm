using System;
using System.Collections.Generic;

namespace DOMAIN.Models;

public partial class Companyemployees
{
    public Guid CompanyEmployeeId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid UserId { get; set; }

    public int? CompanyRoleId { get; set; }

    public string? Title { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? JoinedAt { get; set; }

    public DateTime? LeftAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Companies Company { get; set; } = null!;

    public virtual Companyroles? CompanyRole { get; set; }

    public virtual Users User { get; set; } = null!;
}
