using System.Security.Claims;
using UserService.Models;

namespace UserService.Interfaces;

public interface IJwtProvider
{
    string GenerateJwtToken(User user);
    string GenerateRefreshToken(User user, RefreshToken? savedRefreshToken = null);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string tokensJwtToken);
}