using UserService.Models;

namespace UserService.Interfaces;

public interface IJwtProvider
{
    string GenerateJwtToken(User user);
}