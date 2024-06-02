using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommentService.Models;

public class Review
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; } = null!;
    public required string AuthorId { get; set; }
    public required string MapDiscussionId { get; set; }
    
    public required string Body { get; set; }
    public bool IsEdited { get; set; }
    public bool IsDeleted { get; set; }

    public List<string> CommentIds { get; set; } = new();
    
    public List<OrderedThreadItem> Replies { get; set; } = new();
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EditedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}