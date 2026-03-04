using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Message
{
    public class PaginatedMessagesDto
    {
        public int Total { get; set; }
        public List<MessageDto> Items { get; set; } = new();
    }
}
