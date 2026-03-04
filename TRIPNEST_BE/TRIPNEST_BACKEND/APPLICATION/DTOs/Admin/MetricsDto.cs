using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Admin
{
    public class MetricsDto
    {
        public Dictionary<string, object> Counters { get; set; } = new();
        public Dictionary<string, object> Gauges { get; set; } = new();
    }
}
