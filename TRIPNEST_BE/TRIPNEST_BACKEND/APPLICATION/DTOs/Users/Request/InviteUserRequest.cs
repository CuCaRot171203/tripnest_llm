using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Users.Request
{
    public class InviteUserRequest
    {
        public string Email { get; set; } = null!;
        public Guid CompanyId { get; set; }
        public string Role { get; set; } = null!;
    }
}
