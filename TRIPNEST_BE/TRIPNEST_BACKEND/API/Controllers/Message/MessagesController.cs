using APPLICATION.DTOs.Message;
using APPLICATION.Interfaces.Message;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Message
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _service;

        public MessagesController(IMessageService service)
        {
            _service = service;
        }

        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations([FromQuery] Guid userId, [FromQuery] long? propertyId, CancellationToken ct)
        {
            var conv = await _service.GetConversationsAsync(userId, propertyId, ct);
            return Ok(conv);
        }

        [HttpGet("{conversationId}/messages")]
        public async Task<IActionResult> GetMessages([FromRoute] Guid conversationId, [FromQuery] int limit = 50, [FromQuery] int offset = 0, CancellationToken ct = default)
        {
            var page = await _service.GetMessagesForConversationAsync(conversationId, limit, offset, ct);
            return Ok(page);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage([FromBody] CreateMessageRequestDto dto, CancellationToken ct)
        {
            var created = await _service.CreateMessageAsync(dto, ct);
            return CreatedAtAction(nameof(GetMessages), new { conversationId = created.FromUserId ?? Guid.Empty }, created);
        }

        [HttpPost("moderate/{messageId}")]
        [Authorize(Roles = "Host,Admin,Moderator")]
        public async Task<IActionResult> Moderate(Guid messageId, [FromBody] ModerateMessageRequestDto dto, CancellationToken ct)
        {
            await _service.ModerateMessageAsync(messageId, dto, ct);
            return NoContent();
        }

        [HttpGet("{messageId}")]
        public async Task<IActionResult> GetById(Guid messageId, CancellationToken ct)
        {
            var m = await _service.GetByIdAsync(messageId, ct);
            if (m is null) return NotFound();
            return Ok(m);
        }
    }
}
