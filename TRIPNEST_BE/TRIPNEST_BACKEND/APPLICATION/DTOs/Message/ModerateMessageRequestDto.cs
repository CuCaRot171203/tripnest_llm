using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Message
{
    public class ModerateMessageRequestDto
    {
        public Guid ModeratorId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public bool RemoveMessage { get; set; } = false; 
    }
}
