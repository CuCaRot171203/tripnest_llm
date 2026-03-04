using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Companies
{
    public class EmployeeUpdateDto
    {
        public int? CompanyRoleId { get; set; }
        public bool? IsActive { get; set; }
        public string? Title { get; set; }
    }
}
