using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Companies
{
    public class EmployeeDto
    {
        public Guid CompanyEmployeeId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
        public int? CompanyRoleId { get; set; }
        public string? Title { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? JoinedAt { get; set; }
    }
}
