using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Contracts.Auth;

namespace UserService.Models;

public class User
{
    
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required string Id { get; set; }
    public ICollection<Role> Roles { get; set; } = new List<Role>() {Role.User};
    
}