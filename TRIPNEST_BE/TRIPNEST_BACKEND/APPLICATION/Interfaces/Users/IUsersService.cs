using APPLICATION.DTOs.Users.Request;
using APPLICATION.DTOs.Users.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Interfaces.Users
{
    public interface IUsersService
    {
        Task<UserResponse> GetCurrentUserAsync(Guid currentUserId);
        Task<UserResponse> UpdateCurrentUserAsync(Guid currentUserId, UpdateProfileRequest request);
        Task<UserResponse?> GetPublicProfileAsync(Guid id);
        Task InviteUserAsync(Guid callerUserId, InviteUserRequest request);
        Task<IEnumerable<BookingSummaryResponse>> GetUserBookingsAsync(Guid callerUserId, Guid targetUserId);
    }
}
