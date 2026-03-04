using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Worker
{
    public class SendRemindersResultDto
    {
        public int RemindersCreated { get; set; }
        public int EmailsSent { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}
