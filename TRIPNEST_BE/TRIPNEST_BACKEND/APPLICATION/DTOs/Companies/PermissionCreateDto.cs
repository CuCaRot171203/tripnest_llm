using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Companies
{
    public class PermissionCreateDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}
