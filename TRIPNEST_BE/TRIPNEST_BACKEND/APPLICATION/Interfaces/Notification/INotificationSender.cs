using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Interfaces.Notification
{
    public interface INotificationSender
    {
        Task SendToUserAsync(string userId, string method, object payload, CancellationToken ct = default);
        Task SendToGroupAsync(string groupName, string method, object payload, CancellationToken ct = default);
        Task BroadcastAsync(string method, object payload, CancellationToken ct = default);
    }
}
