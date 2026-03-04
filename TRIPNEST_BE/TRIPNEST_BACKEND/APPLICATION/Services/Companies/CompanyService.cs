using APPLICATION.DTOs.Companies;
using APPLICATION.Interfaces.Companies;
using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.Companies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Services.Companies
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _repo;

        public CompanyService(ICompanyRepository repo)
        {
            _repo = repo;
        }

        private CompanyDto MapToDto(DOMAIN.Models.Companies c) => new CompanyDto
        {
            CompanyId = c.CompanyId,
            Name = c.Name,
            Slug = c.Slug,
            Address = c.Address,
            Phone = c.Phone,
            Email = c.Email,
            OwnerUserId = c.OwnerUserId,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        };

        public async Task<CompanyDto> CreateCompanyAsync(CompanyCreateDto dto)
        {
            var entity = new DOMAIN.Models.Companies
            {
                Name = dto.Name,
                Slug = dto.Slug,
                Address = dto.Address,
                Phone = dto.Phone,
                Email = dto.Email,
                OwnerUserId = dto.OwnerUserId,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repo.CreateAsync(entity);
            return MapToDto(created);
        }

        public async Task<CompanyDto?> GetCompanyAsync(Guid companyId) 
        { 
            var c = await _repo.GetByIdAsync(companyId);
            if (c == null)
            {
                return null;
            }
            return MapToDto(c);
        }

        public async Task<bool> UpdateCompanyAsync(Guid companyId, CompanyUpdateDto dto)
        {
            var c = await _repo.GetByIdAsync(companyId);
            if (c == null)
            {
                return false;
            }
            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                c.Name = dto.Name;
            }
            if (dto.Slug != null)
            {
                c.Slug = dto.Slug;
            }
            if (dto.Address != null)
            {
                c.Address = dto.Address;
            }
            if (dto.Phone != null)
            {
                c.Phone = dto.Phone;
            }
            if (dto.Email != null)
            {
                c.Email = dto.Email;
            }
            if (dto.OwnerUserId.HasValue)
            {
                c.OwnerUserId = dto.OwnerUserId;
            }
            await _repo.UpdateAsync(c);
            return true;
        }

        public async Task<bool> SoftDeleteCompanyAsync(Guid companyId)
        {
            var c = await _repo.GetByIdAsync(companyId);
            if(c == null)
            {
                return false;
            }
            await _repo.SoftDeleteAsync(c);
            return true;
        }

        public async Task<IEnumerable<EmployeeDto>> GetEmployeesAsync(Guid companyId)
        {
            var emps = await _repo.GetEmployeesAsync(companyId);
            return emps.Select(e => new EmployeeDto
            {
                CompanyEmployeeId = e.CompanyEmployeeId,
                CompanyId = e.CompanyId,
                UserId = e.UserId,
                CompanyRoleId = e.CompanyRoleId,
                Title = e.Title,
                IsActive = e.IsActive,
                JoinedAt = e.JoinedAt
            }).ToList();
        }

        public async Task<EmployeeDto?> AddEmployeeAsync(Guid companyId, EmployeeCreateDto dto)
        {
            if (!dto.UserId.HasValue)
            {
                return null;
            }

            var e = new Companyemployees
            {
                CompanyId = companyId,
                UserId = dto.UserId.Value,
                CompanyRoleId = dto.CompanyRoleId,
                Title = dto.Title,
                IsActive = true,
                JoinedAt = DateTime.UtcNow
            };

            var added = await _repo.AddEmployeeAsync(e);
            return new EmployeeDto
            {
                CompanyEmployeeId = added.CompanyEmployeeId,
                CompanyId = added.CompanyId,
                UserId = added.UserId,
                CompanyRoleId = added.CompanyRoleId,
                Title = added.Title,
                IsActive = added.IsActive,
                JoinedAt = added.JoinedAt
            };
        }

        public async Task<bool> UpdateEmployeeAsync(Guid companyId, Guid employeeId, EmployeeUpdateDto dto)
        {
            var e = await _repo.GetEmployeeByIdAsync(companyId, employeeId);
            if(e == null)
            {
                return false;
            }
            await _repo.SoftRemoveEmployeeAsync(e);
            return true;
        }

        public async Task<bool> RemoveEmployeeAsync(Guid companyId, Guid employeeId)
        {
            var e = await _repo.GetEmployeeByIdAsync(companyId, employeeId);
            if (e == null) return false;
            await _repo.SoftRemoveEmployeeAsync(e);
            return true;
        }

        public async Task<IEnumerable<RoleDto>> GetRolesAsync(Guid companyId)
        {
            var roles = await _repo.GetRolesAsync(companyId);
            return roles.Select(r => new RoleDto
            {
                CompanyRoleId = r.CompanyRoleId,
                CompanyId = r.CompanyId,
                Name = r.Name,
                Description = r.Description,
                CreatedAt = r.CreatedAt
            }).ToList();
        }

        public async Task<RoleDto> CreateRoleAsync(Guid companyId, RoleCreateDto dto)
        {
            var r = new Companyroles
            {
                CompanyId = companyId,
                Name = dto.Name,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repo.AddRoleAsync(r);
            return new RoleDto
            {
                CompanyRoleId = created.CompanyRoleId,
                CompanyId = created.CompanyId,
                Name = created.Name,
                Description = created.Description,
                CreatedAt = created.CreatedAt
            };
        }

        public async Task<IEnumerable<PermissionDto>> GetPermissionsAsync(Guid companyId)
        {
            var perms = await _repo.GetPermissionsAsync(companyId);
            return perms.Select(p => new PermissionDto
            {
                PermissionId = p.PermissionId,
                CompanyId = p.CompanyId,
                Name = p.Name,
                Description = p.Description
            }).ToList();
        }

        public async Task<IEnumerable<PermissionDto>> CreatePermissionsAsync(Guid companyId, IEnumerable<PermissionCreateDto> dtos)
        {
            var entities = dtos.Select(d => new Companypermissions
            {
                CompanyId = companyId,
                Name = d.Name,
                Description = d.Description
            }).ToList();

            var created = await _repo.AddPermissionsAsync(entities);

            return created.Select(p => new PermissionDto
            {
                PermissionId = p.PermissionId,
                CompanyId = p.CompanyId,
                Name = p.Name,
                Description = p.Description
            }).ToList();
        }
    }
}
