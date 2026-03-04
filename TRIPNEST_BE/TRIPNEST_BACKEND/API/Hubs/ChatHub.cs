using APPLICATION.DTOs.Message;
using APPLICATION.Interfaces.Message;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;

        public ChatHub(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task SendMessage(CreateMessageRequestDto dto)
        {
            var created = await _messageService.CreateMessageAsync(dto);
            await Clients.Caller.SendAsync("MessageSentAck", created);
        }
    }
}
