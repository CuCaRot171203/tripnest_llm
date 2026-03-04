using System;
using System.Collections.Generic;

namespace DOMAIN.Models;

public partial class Companyroles
{
    public int CompanyRoleId { get; set; }

    public Guid CompanyId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Companies Company { get; set; } = null!;

    public virtual ICollection<Companyemployees> Companyemployees { get; set; } = new List<Companyemployees>();

    public virtual ICollection<Companyrolepermissions> Companyrolepermissions { get; set; } = new List<Companyrolepermissions>();
}
