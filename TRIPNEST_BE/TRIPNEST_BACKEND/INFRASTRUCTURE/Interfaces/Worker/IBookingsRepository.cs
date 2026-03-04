using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Interfaces.Worker
{
    public interface IBookingsRepository
    {
        Task<List<DOMAIN.Models.Bookings>> GetUpcomingBookingsForRemindersAsync(DateTime forDateFromUtc, DateTime forDateToUtc, CancellationToken ct = default);
    }
}
