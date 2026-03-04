using APPLICATION.DTOs.Auth.Request;
using APPLICATION.DTOs.Auth.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> RefreshTokenAsync(RefreshRequest request);
        Task LogoutAsync(LogoutRequest request, Guid userId);
        Task ForgotPasswordAsync(ForgotPasswordRequest request);
        Task ResetPasswordASync(ResetPasswordRequest request);
    }
}
