using APPLICATION.DTOs.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Interfaces.Message
{
    public interface IMessageService
    {
        Task<MessageDto> CreateMessageAsync(CreateMessageRequestDto dto, CancellationToken ct = default);
        Task<PaginatedMessagesDto> GetMessagesForConversationAsync(Guid conversationId, int limit = 50, int offset = 0, CancellationToken ct = default);
        Task<ConversationDto[]> GetConversationsAsync(Guid userId, long? propertyId = null, CancellationToken ct = default);
        Task ModerateMessageAsync(Guid messageId, ModerateMessageRequestDto dto, CancellationToken ct = default);
        Task<MessageDto?> GetByIdAsync(Guid messageId, CancellationToken ct = default);
    }
}
