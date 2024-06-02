using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CommentService.Models;

public class Reply
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }
    public required string AuthorId { get; set; }
    [ForeignKey(nameof(Comment))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CommentId { get; set; } // used when its a reply to a comment
    [ForeignKey(nameof(Review))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ReviewId { get; set; } // used when its a reply to a review
    
    public required string Body { get; set; }
    
    public bool IsEdited { get; set; } = false;
    public bool IsDeleted { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EditedAt { get; set; } = null;
    public DateTime? DeletedAt { get; set; } = null;
    
}