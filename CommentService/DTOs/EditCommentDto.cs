using CommentService.Models;

namespace CommentService.DTOs;

public class EditCommentDto
{
    public required string Id { get; set; }
    public string? Body { get; set; }
    public string? ImageLink { get; set; } // TODO: temp, make our image hosting
    public float? BeatNumber { get; set; } // no beat number means the comment should be viewed as a general comment
    public CommentType? Type { get; set; }
}