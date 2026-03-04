using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Admin
{
    public class GetUsersQueryDto
    {
        public IEnumerable<AdminUserDto> Items { get; set; } = Enumerable.Empty<AdminUserDto>();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
