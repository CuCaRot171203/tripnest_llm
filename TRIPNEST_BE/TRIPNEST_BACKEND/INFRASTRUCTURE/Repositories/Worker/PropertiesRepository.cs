using DOMAIN;
using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.Worker;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Repositories.Worker
{
    public class PropertiesRepository : IPropertiesRepository
    {
        private readonly TripnestDbContext _db;

        public PropertiesRepository(TripnestDbContext db)
        {
            _db = db;
        }

        public async Task<Properties?> GetByIdAsync(
            long propertyId, 
            CancellationToken ct = default)
        {
            return await _db.Properties
                .Include(p => p.Propertyphotos)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PropertyId == propertyId, ct);
        }
    }
}
