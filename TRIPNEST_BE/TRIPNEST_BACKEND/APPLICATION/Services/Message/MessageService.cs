using APPLICATION.DTOs.Message;
using APPLICATION.Interfaces.Message;
using APPLICATION.Interfaces.Notification;
using AutoMapper;
using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace APPLICATION.Services.Message
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _repo;
        private readonly IMapper _mapper;
        private readonly INotificationSender _notifier;

        public MessageService(
            IMessageRepository repo, 
            IMapper mapper, 
            INotificationSender notifier)
        {
            _repo = repo;
            _mapper = mapper;
            _notifier = notifier;
        }

        public async Task<MessageDto> CreateMessageAsync(
            CreateMessageRequestDto dto, 
            CancellationToken ct = default)
        {
            var model = new Messages
            {
                MessageId = Guid.NewGuid(),
                FromUserId = dto.FromUserId,
                ToUserId = dto.ToUserId,
                PropertyId = dto.PropertyId,
                Content = dto.Content,
                IsAi = dto.IsAi,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(model, ct);
            var result = _mapper.Map<MessageDto>(model);

            if (dto.ToUserId.HasValue)
            {
                await _notifier.SendToUserAsync(dto.ToUserId.Value.ToString(), "ReceiveMessage", result, ct);
            }

            if (dto.PropertyId.HasValue)
            {
                var group = $"property-{dto.PropertyId.Value}";
                await _notifier.SendToGroupAsync(group, "ReceiveMessage", result, ct);
            }

            return result;
        }

        public async Task<PaginatedMessagesDto> GetMessagesForConversationAsync(
            Guid conversationId, 
            int limit = 50, 
            int offset = 0, 
            CancellationToken ct = default)
        {
            var (items, total) = await _repo.GetMessagesAsync(conversationId, limit, offset, ct);
            var dto = new PaginatedMessagesDto
            {
                Total = total,
                Items = items.Select(m => _mapper.Map<MessageDto>(m)).ToList()
            };
            return dto;
        }

        public async Task<ConversationDto[]> GetConversationsAsync(Guid userId, long? propertyId = null, CancellationToken ct = default)
        {
            var items = await _repo.GetConversationsAsync(userId, propertyId, ct);

            var grouped = items
                .GroupBy(m => m.PropertyId.HasValue ? $"property-{m.PropertyId}" : $"user-{(m.FromUserId == userId ? m.ToUserId : m.FromUserId)}")
                .Select(g =>
                {
                    var latest = g.OrderByDescending(x => x.CreatedAt).First();
                    return new ConversationDto
                    {
                        ConversationId = g.Key,
                        WithUserId = latest.FromUserId == userId ? latest.ToUserId : latest.FromUserId,
                        PropertyId = latest.PropertyId,
                        LastMessage = latest.Content,
                        LastAt = latest.CreatedAt
                    };
                })
                .OrderByDescending(c => c.LastAt)
                .ToArray();

            return grouped;
        }

        public async Task ModerateMessageAsync(Guid messageId, ModerateMessageRequestDto dto, CancellationToken ct = default)
        {
            var message = await _repo.GetByIdAsync(messageId, ct);
            if (message == null)
            {
                throw new KeyNotFoundException("Message not found");
            }
            var before = JsonSerializer.Serialize(message);
            if (dto.RemoveMessage)
            {
                message.Content = "[removed by moderator]";
            }
            await _repo.UpdateAsync(message, ct);

            var after = JsonSerializer.Serialize(message);
            var audit = new Auditlogs
            {
                AuditId = Guid.NewGuid(),
                EntityType = nameof(Messages),
                EntityId = message.MessageId.ToString(),
                Action = "ModerateMessage",
                BeforeJson = before,
                AfterJson = after,
                PerformedBy = dto.ModeratorId,
                PerformedAt = DateTime.UtcNow
            };
            await _repo.AddAuditLogAsync(audit, ct);
            if (message.ToUserId.HasValue)
            {
                await _notifier.SendToUserAsync(
                    message.ToUserId.Value.ToString(), 
                    "MessageModerated", 
                    new { messageId = message.MessageId }, ct);
            }

            if (message.FromUserId.HasValue)
            {
                await _notifier.SendToUserAsync(
                    message.FromUserId.Value.ToString(), 
                    "MessageModerated", 
                    new { messageId = message.MessageId }, ct);
            }
        }

        public async Task<MessageDto?> GetByIdAsync(Guid messageId, CancellationToken ct = default)
        {
            var m = await _repo.GetByIdAsync(messageId, ct);
            return m == null ? null : _mapper.Map<MessageDto>(m);
        }
    }
}
