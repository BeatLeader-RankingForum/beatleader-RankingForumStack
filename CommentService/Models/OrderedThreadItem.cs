using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommentService.Models;

public class OrderedThreadItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }
    public required string AuthorId { get; set; }
    [ForeignKey(nameof(Comment))]
    public required string CommentId { get; set; }
    
    public int Position { get; set; }
    public bool IsEdited { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EditedAt { get; set; } = null;
}