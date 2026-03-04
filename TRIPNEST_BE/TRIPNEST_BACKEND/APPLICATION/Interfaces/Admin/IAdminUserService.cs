using APPLICATION.DTOs.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Interfaces.Admin
{
    public interface IAdminUserService
    {
        Task<GetUsersQueryDto> GetUsersAsync(int? roleId, bool? isActive, int page = 1, int pageSize = 50, CancellationToken ct = default);
        Task<AdminUserDto?> GetByIdAsync(Guid userId, CancellationToken ct = default);
        Task<AdminUserDto?> UpdateUserRoleAsync(Guid userId, int? roleId, CancellationToken ct = default);
    }
}
