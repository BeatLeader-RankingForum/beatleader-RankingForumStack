using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Models;

public class RefreshToken
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; } = null!;

    [ForeignKey(nameof(User))]
    public required string UserId { get; set; }
    public required string Token { get; set; }
    public DateTime ExpiryDate { get; set; }
}