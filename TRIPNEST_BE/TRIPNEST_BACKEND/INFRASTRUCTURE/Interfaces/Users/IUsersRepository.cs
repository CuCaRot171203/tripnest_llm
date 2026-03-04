using DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Interfaces.Users
{
    public interface IUsersRepository
    {
        Task<DOMAIN.Models.Users?> GetByIdAsync(Guid id);
        Task<DOMAIN.Models.Users?> GetByEmailAsync(string email);
        Task UpdateProfileAsync(DOMAIN.Models.Users users);
        Task CreateUnverifiedUserAsync(string email, Guid userId);
        Task AddCompanyEmployeeAsync(Guid companyId, Guid userId, string role);
        Task<IEnumerable<DOMAIN.Models.Bookings>> GetBookingsForUserAsync(Guid userId);
    }
}
