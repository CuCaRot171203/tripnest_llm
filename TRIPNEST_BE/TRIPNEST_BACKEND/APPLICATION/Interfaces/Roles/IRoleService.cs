using APPLICATION.DTOs.Roles.Request;
using APPLICATION.DTOs.Roles.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Interfaces.Roles
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleResponse>> GetAllAsync();
        Task<RoleResponse> CreateAsync(RoleCreateRequest request);
    }
}
