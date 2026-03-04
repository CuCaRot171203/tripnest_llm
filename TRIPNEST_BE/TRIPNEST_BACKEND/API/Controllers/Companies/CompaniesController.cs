using APPLICATION.DTOs.Companies;
using APPLICATION.Interfaces.Companies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Companies
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _service;

        public CompaniesController(ICompanyService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Superuser,Provider")]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var created = await _service.CreateCompanyAsync(dto);

            return CreatedAtAction(nameof(GetCompany), new { companyId = created.CompanyId }, created);
        }

        [HttpGet("{companyId:guid}")]
        [Authorize]
        public async Task<IActionResult> GetCompany([FromRoute] Guid companyId)
        {
            var c = await _service.GetCompanyAsync(companyId);
            if (c == null) return NotFound();
            return Ok(c);
        }

        [HttpPut("{companyId:guid}")]
        [Authorize(Roles = "CompanyOwner,Admin")]
        public async Task<IActionResult> UpdateCompany([FromRoute] Guid companyId, [FromBody] CompanyUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var ok = await _service.UpdateCompanyAsync(companyId, dto);
            if (!ok) return NotFound();
            return Ok(new { message = "Company updated" });
        }

        [HttpDelete("{companyId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCompany([FromRoute] Guid companyId)
        {
            var ok = await _service.SoftDeleteCompanyAsync(companyId);
            if (!ok) return NotFound();
            return NoContent(); // 204
        }

        [HttpGet("{companyId:guid}/employees")]
        [Authorize(Roles = "CompanyManager,CompanyOwner,Admin")]
        public async Task<IActionResult> GetEmployees([FromRoute] Guid companyId)
        {
            var emps = await _service.GetEmployeesAsync(companyId);
            return Ok(emps);
        }

        [HttpPost("{companyId:guid}/employees")]
        [Authorize(Roles = "CompanyManager,CompanyOwner")]
        public async Task<IActionResult> AddEmployee([FromRoute] Guid companyId, [FromBody] EmployeeCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _service.AddEmployeeAsync(companyId, dto);
            if (created == null)
            {
                return BadRequest(new { message = "Either provide existing userId or implement invitation flow." });
            }

            return CreatedAtAction(nameof(GetEmployees), new { companyId = companyId }, created);
        }

        [HttpPut("{companyId:guid}/employees/{employeeId:guid}")]
        [Authorize(Roles = "CompanyManager,CompanyOwner")]
        public async Task<IActionResult> UpdateEmployee([FromRoute] Guid companyId, [FromRoute] Guid employeeId, [FromBody] EmployeeUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var ok = await _service.UpdateEmployeeAsync(companyId, employeeId, dto);
            if (!ok) return NotFound();
            return Ok(new { message = "Employee updated" });
        }

        [HttpDelete("{companyId:guid}/employees/{employeeId:guid}")]
        [Authorize(Roles = "CompanyManager,CompanyOwner")]
        public async Task<IActionResult> RemoveEmployee([FromRoute] Guid companyId, [FromRoute] Guid employeeId)
        {
            var ok = await _service.RemoveEmployeeAsync(companyId, employeeId);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpGet("{companyId:guid}/roles")]
        [Authorize(Roles = "CompanyManager,CompanyOwner")]
        public async Task<IActionResult> GetRoles([FromRoute] Guid companyId)
        {
            var roles = await _service.GetRolesAsync(companyId);
            return Ok(roles);
        }

        [HttpPost("{companyId:guid}/roles")]
        [Authorize(Roles = "CompanyOwner,Admin")]
        public async Task<IActionResult> CreateRole([FromRoute] Guid companyId, [FromBody] RoleCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _service.CreateRoleAsync(companyId, dto);
            return CreatedAtAction(nameof(GetRoles), new { companyId = companyId }, created);
        }

        [HttpPost("{companyId:guid}/permissions")]
        [Authorize(Roles = "CompanyOwner")]
        public async Task<IActionResult> CreatePermissions([FromRoute] Guid companyId, [FromBody] IEnumerable<PermissionCreateDto> dtos)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _service.CreatePermissionsAsync(companyId, dtos);
            return CreatedAtAction(nameof(GetPermissions), new { companyId = companyId }, created);
        }

        [HttpGet("{companyId:guid}/permissions")]
        [Authorize(Roles = "CompanyOwner,CompanyManager")]
        public async Task<IActionResult> GetPermissions([FromRoute] Guid companyId)
        {
            var perms = await _service.GetPermissionsAsync(companyId);
            return Ok(perms);
        }
    }
}
