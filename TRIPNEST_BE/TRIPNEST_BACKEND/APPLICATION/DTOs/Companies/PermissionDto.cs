using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Companies
{
    public class PermissionDto
    {
        public int PermissionId { get; set; }
        public Guid CompanyId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}
