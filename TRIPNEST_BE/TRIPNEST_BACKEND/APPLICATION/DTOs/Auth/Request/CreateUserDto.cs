using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Auth.Request
{
    public class CreateUserDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Locale { get; set; }
        public bool? IsActive { get; set; }
    }
}
