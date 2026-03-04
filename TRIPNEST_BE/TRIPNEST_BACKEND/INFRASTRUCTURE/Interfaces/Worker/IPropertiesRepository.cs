using DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Interfaces.Worker
{
    public interface IPropertiesRepository
    {
        Task<Properties?> GetByIdAsync(long propertyId, CancellationToken ct = default);
    }
}
