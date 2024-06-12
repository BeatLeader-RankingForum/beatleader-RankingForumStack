using CommentService.Models;

namespace CommentService.DTOs;

public class AddCommentDto
{
    public required string MapDiscussionId { get; set; }
    public string? DifficultyDiscussionId { get; set; } // if null then it's a general comment for all difficulties
    //public required int DifficultyVersion { get; set; } TODO: implement

    public required string Body { get; set; }
    public string? ImageLink { get; set; } // TODO: temp, make our image hosting
    public float? BeatNumber { get; set; } // no beat number means the comment should be viewed as a general comment
    public CommentType Type { get; set; }
}