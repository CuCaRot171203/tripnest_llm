using DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Interfaces.Message
{
    public interface IMessageRepository
    {
        Task AddAsync(Messages message, CancellationToken ct = default);
        Task<Messages?> GetByIdAsync(Guid messageId, CancellationToken ct = default);
        Task<(List<Messages> items, int total)> GetMessagesAsync(Guid conversationId, int limit, int offset, CancellationToken ct = default);
        Task<List<Messages>> GetConversationsAsync(Guid userId, long? propertyId, CancellationToken ct = default);
        Task UpdateAsync(Messages message, CancellationToken ct = default);
        Task AddAuditLogAsync(Auditlogs log, CancellationToken ct = default);
    }
}
