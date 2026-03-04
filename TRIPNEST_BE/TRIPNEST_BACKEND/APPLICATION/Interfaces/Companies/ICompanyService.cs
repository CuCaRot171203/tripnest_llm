using APPLICATION.DTOs.Companies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Interfaces.Companies
{
    public interface ICompanyService
    {
        Task<CompanyDto?> GetCompanyAsync(Guid companyId);
        Task<CompanyDto> CreateCompanyAsync(CompanyCreateDto dto);
        Task<bool> UpdateCompanyAsync(Guid companyId, CompanyUpdateDto dto);
        Task<bool> SoftDeleteCompanyAsync(Guid companyId);
        Task<IEnumerable<EmployeeDto>> GetEmployeesAsync(Guid companyId);
        Task<EmployeeDto?> AddEmployeeAsync(Guid companyId, EmployeeCreateDto dto);
        Task<bool> UpdateEmployeeAsync(Guid companyId, Guid employeeId, EmployeeUpdateDto dto);
        Task<bool> RemoveEmployeeAsync(Guid companyId, Guid employeeId);
        Task<IEnumerable<RoleDto>> GetRolesAsync(Guid companyId);
        Task<RoleDto> CreateRoleAsync(Guid companyId, RoleCreateDto dto);
        Task<IEnumerable<PermissionDto>> GetPermissionsAsync(Guid companyId);
        Task<IEnumerable<PermissionDto>> CreatePermissionsAsync(Guid companyId, IEnumerable<PermissionCreateDto> dtos);
    }
}
