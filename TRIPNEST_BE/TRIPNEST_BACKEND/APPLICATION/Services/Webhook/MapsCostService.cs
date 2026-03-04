using APPLICATION.DTOs.Webhook;
using APPLICATION.Interfaces.Webhook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Services.Webhook
{
    public class MapsCostService : IMapsCostService
    {
        public MapsCostService()
        {
        }

        public Task<bool> ProcessMapsCostAsync(MapsCostWebhookDto dto, CancellationToken ct = default)
        {
            // For demo: just log or persist as needed.
            // TODO: persist to DB if you have table e.g. MapsCosts
            return Task.FromResult(true);
        }
    }
}
