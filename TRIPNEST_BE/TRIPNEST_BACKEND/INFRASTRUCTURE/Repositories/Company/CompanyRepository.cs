using DOMAIN;
using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.Companies;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Repositories.Company
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly TripnestDbContext _db;

        public CompanyRepository(TripnestDbContext db)
        {
            _db = db;
        }

        public async Task<Companies?> GetByIdAsync(Guid companyId)
        {
            return await _db.Companies
                .Include(c => c.Companyemployees)
                .Include(c => c.Companyroles)
                .Include(c => c.Companypermissions)
                .FirstOrDefaultAsync(c => c.CompanyId == companyId);
        }

        public async Task<Companies> CreateAsync(Companies company)
        {
            company.CompanyId = Guid.NewGuid();
            company.CreatedAt = DateTime.UtcNow;
            _db.Companies.Add(company);
            await _db.SaveChangesAsync();
            return company;
        }

        public async Task UpdateAsync(Companies company)
        {
            company.UpdatedAt = DateTime.UtcNow;
            _db.Companies.Update(company);
            await _db.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(Companies company)
        {
            company.Name = company.Name + "_deleted_";
            company.UpdatedAt = DateTime.UtcNow;
            _db.Companies.Update(company);

            var employees = await _db.Companyemployees
                .Where(e => e.CompanyId == company.CompanyId)
                .ToListAsync();

            foreach(var e in employees)
            {
                e.IsActive = false;
                e.LeftAt = DateTime.UtcNow;
                _db.Companyemployees.Update(e);
            }

            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Companyemployees>> GetEmployeesAsync(Guid companyId)
        {
            return await _db.Companyemployees
                .Include(e => e.User)
                .Include(e => e.CompanyRole)
                .Where(e => e.CompanyId == companyId && (e.IsActive == null || e.IsActive.Value))
                .ToListAsync();
        }

        public async Task<Companyemployees?> GetEmployeeByIdAsync(Guid companyId, Guid employeeId)
        {
            return await _db.Companyemployees
                .Include(e => e.User)
                .Include(e => e.CompanyRole)
                .FirstOrDefaultAsync(e => e.CompanyId == companyId && e.CompanyEmployeeId == employeeId);
        }

        public async Task<Companyemployees> AddEmployeeAsync(Companyemployees employee)
        {
            employee.CompanyEmployeeId = Guid.NewGuid();
            employee.CreatedAt = DateTime.UtcNow;
            employee.JoinedAt = DateTime.UtcNow;
            employee.IsActive = true;
            _db.Companyemployees.Add(employee);
            await _db.SaveChangesAsync();
            return employee;
        }

        public async Task UpdateEmployeeAsync(Companyemployees employee)
        {
            employee.UpdatedAt = DateTime.UtcNow;
            _db.Companyemployees.Update(employee);
            await _db.SaveChangesAsync();
        }

        public async Task SoftRemoveEmployeeAsync(Companyemployees employee)
        {
            employee.IsActive = false;
            employee.LeftAt = DateTime.UtcNow;
            employee.UpdatedAt = DateTime.UtcNow;
            _db.Companyemployees.Update(employee);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Companyroles>> GetRolesAsync(Guid companyId)
        {
            return await _db.Companyroles
                .Where(r => r.CompanyId == companyId)
                .ToListAsync();
        }

        public async Task<Companyroles> AddRoleAsync(Companyroles role)
        {
            role.CreatedAt = DateTime.UtcNow;
            _db.Companyroles.Add(role);
            await _db.SaveChangesAsync();
            return role;
        }

        public async Task<IEnumerable<Companypermissions>> AddPermissionsAsync(IEnumerable<Companypermissions> perms)
        {
            foreach (var p in perms)
            {
                _db.Companypermissions.Add(p);
            }
            await _db.SaveChangesAsync();
            return perms;
        }

        public async Task<IEnumerable<Companypermissions>> GetPermissionsAsync(Guid companyId)
        {
            return await _db.Companypermissions.Where(p => p.CompanyId == companyId).ToListAsync();
        }
    }
}
