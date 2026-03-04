using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Webhook
{
    public class MapsCostWebhookDto
    {
        public string? Provider { get; set; }
        public long? PropertyId { get; set; }
        public decimal? Cost { get; set; }
        public string? Currency { get; set; }
        public IDictionary<string, string>? Metadata { get; set; }
    }
}
