using System;
using System.Collections.Generic;

namespace DOMAIN.Models;

public partial class Companies
{
    public Guid CompanyId { get; set; }

    public string Name { get; set; } = null!;

    public string? Slug { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public Guid? OwnerUserId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Companyemployees> Companyemployees { get; set; } = new List<Companyemployees>();

    public virtual ICollection<Companypermissions> Companypermissions { get; set; } = new List<Companypermissions>();

    public virtual ICollection<Companyroles> Companyroles { get; set; } = new List<Companyroles>();

    public virtual Users? OwnerUser { get; set; }
}
