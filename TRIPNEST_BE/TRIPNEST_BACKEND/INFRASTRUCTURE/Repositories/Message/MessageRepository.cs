using DOMAIN;
using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.Message;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Repositories.Message
{
    public class MessageRepository : IMessageRepository
    {
        private readonly TripnestDbContext _db;

        public MessageRepository(TripnestDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Messages message, CancellationToken ct = default)
        {
            if (message.MessageId == Guid.Empty)
            {
                message.MessageId = Guid.NewGuid();
            }
            message.CreatedAt = DateTime.UtcNow;
            _db.Messages.Add(message);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<Messages?> GetByIdAsync(Guid messageId, CancellationToken ct = default)
        {
            return await _db.Messages
                .AsNoTracking()
                .Include(m => m.FromUser)
                .Include(m => m.ToUser)
                .FirstOrDefaultAsync(m => m.MessageId == messageId, ct);
        }

        public async Task<(List<Messages> items, int total)> GetMessagesAsync(Guid conversationId, int limit, int offset, CancellationToken ct = default)
        {
            var q = _db.Messages
                .AsNoTracking()
                .Where(m => m.FromUserId == conversationId || m.ToUserId == conversationId)
                .OrderByDescending(m => m.CreatedAt);

            var total = await q.CountAsync(ct);
            var items = await q.Skip(offset).Take(limit).OrderBy(m => m.CreatedAt).ToListAsync(ct); // return ascending
            return (items, total);
        }

        public async Task<List<Messages>> GetConversationsAsync(Guid userId, long? propertyId, CancellationToken ct = default)
        {
            var q = _db.Messages
                .AsNoTracking()
                .Where(m => m.FromUserId == userId || m.ToUserId == userId || (propertyId.HasValue && m.PropertyId == propertyId))
                .OrderByDescending(m => m.CreatedAt);

            return await q.Take(100).ToListAsync(ct);
        }

        public async Task UpdateAsync(Messages message, CancellationToken ct = default)
        {
            _db.Messages.Update(message);
            await _db.SaveChangesAsync(ct);
        }

        public async Task AddAuditLogAsync(Auditlogs log, CancellationToken ct = default)
        {
            if (log.AuditId == Guid.Empty) log.AuditId = Guid.NewGuid();
            log.PerformedAt = DateTime.UtcNow;
            _db.Auditlogs.Add(log);
            await _db.SaveChangesAsync(ct);
        }
    }
}
