using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Message
{
    public class MessageDto
    {
        public Guid MessageId { get; set; }
        public Guid? FromUserId { get; set; }
        public Guid? ToUserId { get; set; }
        public long? PropertyId { get; set; }
        public string? Content { get; set; }
        public bool? IsAi { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
