using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Admin
{
    public class AuditLogDto
    {
        public Guid AuditId { get; set; }
        public string? EntityType { get; set; }
        public string? EntityId { get; set; }
        public string? Action { get; set; }
        public string? BeforeJson { get; set; }
        public string? AfterJson { get; set; }
        public Guid? PerformedBy { get; set; }
        public DateTime? PerformedAt { get; set; }
        public string? PerformedByName { get; set; }
    }
}
