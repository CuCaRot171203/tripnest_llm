using DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Interfaces.Companies
{
    public interface ICompanyRepository
    {
        Task<DOMAIN.Models.Companies?> GetByIdAsync(Guid companyId);
        Task<DOMAIN.Models.Companies> CreateAsync(DOMAIN.Models.Companies company);
        Task UpdateAsync(DOMAIN.Models.Companies company);
        Task SoftDeleteAsync(DOMAIN.Models.Companies company);
        Task<IEnumerable<Companyemployees>> GetEmployeesAsync(Guid companyId);
        Task<Companyemployees?> GetEmployeeByIdAsync(Guid companyId, Guid employeeId);
        Task<Companyemployees> AddEmployeeAsync(Companyemployees employee);
        Task UpdateEmployeeAsync(Companyemployees employee);
        Task SoftRemoveEmployeeAsync(Companyemployees employee);
        Task<IEnumerable<Companyroles>> GetRolesAsync(Guid companyId);
        Task<Companyroles> AddRoleAsync(Companyroles role);
        Task<IEnumerable<Companypermissions>> AddPermissionsAsync(IEnumerable<Companypermissions> perms);
        Task<IEnumerable<Companypermissions>> GetPermissionsAsync(Guid companyId);
    }
}
