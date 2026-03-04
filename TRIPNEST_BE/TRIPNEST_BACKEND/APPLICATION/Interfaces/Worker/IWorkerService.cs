using APPLICATION.DTOs.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Interfaces.Worker
{
    public interface IWorkerService
    {
        Task<IndexPropertyResponseDto> IndexPropertyAsync(long propertyId, bool forceReindex = false, CancellationToken ct = default);
        Task<SendRemindersResultDto> SendRemindersAsync(int daysAhead = 1, CancellationToken ct = default);
    }
}
