using APPLICATION.DTOs.Webhook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Interfaces.Webhook
{
    public interface IMapsCostService
    {
        Task<bool> ProcessMapsCostAsync(MapsCostWebhookDto dto, CancellationToken ct = default);
    }
}
