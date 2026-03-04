using System;
using System.Collections.Generic;

namespace DOMAIN.Models;

public partial class Companyrolepermissions
{
    public int Id { get; set; }

    public int CompanyRoleId { get; set; }

    public int PermissionId { get; set; }

    public virtual Companyroles CompanyRole { get; set; } = null!;

    public virtual Companypermissions Permission { get; set; } = null!;
}
