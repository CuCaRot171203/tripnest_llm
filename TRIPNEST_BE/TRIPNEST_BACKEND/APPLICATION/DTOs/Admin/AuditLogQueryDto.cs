using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Admin
{
    public class AuditLogQueryDto
    {
        public string? EntityType { get; set; }
        public string? EntityId { get; set; }
        public Guid? UserId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 25;
        public string SortBy { get; set; } = "PerformedAt";
        public string SortDirection { get; set; } = "desc";
    }
}
