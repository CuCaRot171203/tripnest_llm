using DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Interfaces.Roles
{
    public interface IRoleRepository
    {
        Task<IEnumerable<DOMAIN.Models.Roles>> GetAllAsync();
        Task<DOMAIN.Models.Roles?> GetByNameAsync(string name);
        Task<DOMAIN.Models.Roles> AddAsync(DOMAIN.Models.Roles role);
    }
}
