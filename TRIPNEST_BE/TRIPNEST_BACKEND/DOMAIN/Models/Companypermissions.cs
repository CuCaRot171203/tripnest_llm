using System;
using System.Collections.Generic;

namespace DOMAIN.Models;

public partial class Companypermissions
{
    public int PermissionId { get; set; }

    public Guid CompanyId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual Companies Company { get; set; } = null!;

    public virtual ICollection<Companyrolepermissions> Companyrolepermissions { get; set; } = new List<Companyrolepermissions>();
}
