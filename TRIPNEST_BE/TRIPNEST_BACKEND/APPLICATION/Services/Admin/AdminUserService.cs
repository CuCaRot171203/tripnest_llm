using APPLICATION.DTOs.Admin;
using APPLICATION.Interfaces.Admin;
using INFRASTRUCTURE.Interfaces.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Services.Admin
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IUsersRepository _usersRepo;

        public AdminUserService(IUsersRepository usersRepo)
        {
            _usersRepo = usersRepo;
        }

        public async Task<GetUsersQueryDto> GetUsersAsync(
            int? roleId, bool? isActive, int page = 1, 
            int pageSize = 50, CancellationToken ct = default)
        {
            var (items, total) = await _usersRepo
                .GetUsersAsync(roleId, isActive, page, pageSize, ct);

            var dtos = items.Select(u => ToDto(u)).ToList();

            return new GetUsersQueryDto
            {
                Items = dtos,
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<AdminUserDto?> GetByIdAsync(Guid userId, CancellationToken ct = default)
        {
            var u = await _usersRepo.GetByIdAsync(userId, ct);
            return u == null ? null : ToDto(u);
        }

        public async Task<AdminUserDto?> UpdateUserRoleAsync(Guid userId, int? roleId, CancellationToken ct = default)
        {
            var user = await _usersRepo.GetByIdAsync(userId, ct);
            if (user == null)
            {
                return null;
            }

            user.RoleId = roleId;
            user.UpdatedAt = DateTime.UtcNow;

            await _usersRepo.UpdateAsync(user, ct);
            await _usersRepo.SaveChangesAsync(ct);

            return ToDto(user);
        }

        private AdminUserDto ToDto(DOMAIN.Models.Users u) => new AdminUserDto
        {
            UserId = u.UserId,
            RoleId = u.RoleId,
            Email = u.Email,
            FullName = u.FullName,
            Phone = u.Phone,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt
        };
    }
}
