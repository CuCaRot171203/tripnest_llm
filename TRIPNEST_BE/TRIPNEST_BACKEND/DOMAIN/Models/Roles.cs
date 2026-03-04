using System;
using System.Collections.Generic;

namespace DOMAIN.Models;

public partial class Roles
{
    public int RoleId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Users> Users { get; set; } = new List<Users>();
}
