using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommentService.Models;

public class Comment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; } = null!;
    public required string AuthorId { get; set; }
    public required string MapDiscussionId { get; set; }
    public string? DifficultyDiscussionId { get; set; } // if null then it's a general comment for all difficulties
    //public required int DifficultyVersion { get; set; } TODO: implement
    
    // TODO: consider storing the review id too for easy reference in front-end

    public required string Body { get; set; }
    public string? ImageLink { get; set; } // TODO: temp, make our image hosting
    public float? BeatNumber { get; set; } // no beat number means the comment should be viewed as a general comment
    public CommentType Type { get; set; }
    public bool IsResolved { get; set; }
    public bool IsEdited { get; set; }
    public bool IsDeleted { get; set; }

    public List<Reply> Replies { get; set; } = new();
    public List<StatusUpdate> Events { get; set; } = new();
    
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? EditedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}