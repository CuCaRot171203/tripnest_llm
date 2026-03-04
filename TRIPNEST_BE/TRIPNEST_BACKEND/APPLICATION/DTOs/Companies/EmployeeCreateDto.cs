namespace APPLICATION.DTOs.Companies
{
    public class EmployeeCreateDto
    {
        public Guid? UserId { get; set; }
        public string? Email { get; set; }
        public int? CompanyRoleId { get; set; }
        public string? Title { get; set; }
    }
}
