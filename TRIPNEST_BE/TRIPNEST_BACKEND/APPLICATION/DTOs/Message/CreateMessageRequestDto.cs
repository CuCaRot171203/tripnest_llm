using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Message
{
    public class CreateMessageRequestDto
    {
        public Guid? FromUserId { get; set; }
        public Guid? ToUserId { get; set; }
        public long? PropertyId { get; set; }
        public string? Content { get; set; }
        public bool IsAi { get; set; } = false;
    }
}
