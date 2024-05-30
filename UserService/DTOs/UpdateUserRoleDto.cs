using Contracts.Auth;

namespace UserService.DTOs;

public class UpdateUserRoleDto
{
    public required string Id { get; set; }
    public required Role Role { get; set; }
}