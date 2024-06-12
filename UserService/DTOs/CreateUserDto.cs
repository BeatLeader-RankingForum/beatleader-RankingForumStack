using Contracts.Auth;

namespace UserService.DTOs;

public class CreateUserDto
{
    public required string Id { get; set; }
    public required ICollection<Role> Roles { get; set; }
}