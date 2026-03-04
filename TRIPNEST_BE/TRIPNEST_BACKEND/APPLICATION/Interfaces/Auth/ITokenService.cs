using APPLICATION.DTOs.Auth.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Interfaces.Auth
{
    public interface ITokenService
    {
        string GenerateAccessToken(UserDto user, TimeSpan? lifetime = null);
        RefreshTokenDto GenerateRefreshToken(Guid userId, TimeSpan? lifetime = null);
    }
}
