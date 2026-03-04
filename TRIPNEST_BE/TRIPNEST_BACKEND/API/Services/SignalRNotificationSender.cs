using API.Hubs;
using APPLICATION.Interfaces.Notification;
using Microsoft.AspNetCore.SignalR;

namespace API.Services
{
    public class SignalRNotificationSender : INotificationSender
    {
        private readonly IHubContext<NotificationsHub> _hub;

        public SignalRNotificationSender(IHubContext<NotificationsHub> hub)
        {
            _hub = hub;
        }

        public async Task SendToUserAsync(string userId, string method, object payload, CancellationToken ct = default)
        {
            // Note: SignalR's User identifier is string — make sure you configure ClaimsPrincipal.NameIdentifier
            await _hub.Clients.User(userId).SendAsync(method, payload, ct);
        }

        public async Task SendToGroupAsync(string groupName, string method, object payload, CancellationToken ct = default)
        {
            await _hub.Clients.Group(groupName).SendAsync(method, payload, ct);
        }

        public async Task BroadcastAsync(string method, object payload, CancellationToken ct = default)
        {
            await _hub.Clients.All.SendAsync(method, payload, ct);
        }
    }
}
