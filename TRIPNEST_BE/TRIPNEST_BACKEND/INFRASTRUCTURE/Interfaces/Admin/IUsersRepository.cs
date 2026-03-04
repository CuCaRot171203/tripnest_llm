using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Interfaces.Admin
{
    public interface IUsersRepository
    {
        Task<DOMAIN.Models.Users?> GetByIdAsync(Guid userId, CancellationToken ct = default);
        Task<(IEnumerable<DOMAIN.Models.Users> Items, int Total)> GetUsersAsync(int? roleId, bool? isActive, int page = 1, int pageSize = 50, CancellationToken ct = default);
        Task UpdateAsync(DOMAIN.Models.Users entity, CancellationToken ct = default);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
