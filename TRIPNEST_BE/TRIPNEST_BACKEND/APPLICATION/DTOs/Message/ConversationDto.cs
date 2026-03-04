using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Message
{
    public class ConversationDto
    {
        public string ConversationId { get; set; } = default!;
        public Guid? WithUserId { get; set; }
        public long? PropertyId { get; set; }
        public string? LastMessage { get; set; }
        public DateTime? LastAt { get; set; }
    }
}
