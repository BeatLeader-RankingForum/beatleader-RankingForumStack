using Contracts.Auth;

namespace UserService.Models;

public class User
{
    
    public string Id { get; set; }
    
    public string Username { get; set; }
    
    public ICollection<Role> Roles { get; set; } = new List<Role>() {Role.User};
    
}