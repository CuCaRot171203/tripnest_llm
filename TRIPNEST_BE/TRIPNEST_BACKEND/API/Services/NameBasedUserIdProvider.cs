using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace API.Services
{
    public class NameBasedUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            var id = connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(id))
            {
                return id;
            }
            return connection.User?.FindFirst("sub")?.Value;
        }
    }
}
