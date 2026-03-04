using APPLICATION.DTOs.Roles.Request;
using APPLICATION.DTOs.Roles.Response;
using APPLICATION.Interfaces.Roles;
using DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Services.Roles
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _repo;

        public RoleService(IRoleRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<RoleResponse>> GetAllAsync()
        {
            var roles = await _repo.GetAllAsync();
            return roles.Select(r => new RoleResponse
            {
                RoleId = r.RoleId,
                Name = r.Name,
                Description = r.Description,
                CreatedAt = r.CreatedAt
            });
        }

        public async Task<RoleResponse> CreateAsync(RoleCreateRequest request)
        {
            var existing = await _repo.GetByNameAsync(request.Name.Trim());
            if(existing != null)
            {
                throw new InvalidOperationException($"Role with name '{request.Name}' already exists.");
            }

            var role = new DOMAIN.Models.Roles
            {
                Name = request.Name.Trim(),
                Description = request.Description
            };

            var added = await _repo.AddAsync(role);

            return new RoleResponse
            {
                RoleId = added.RoleId,
                Name = added.Name,
                Description = added.Description,
                CreatedAt = added.CreatedAt
            };
        }
    }
}
