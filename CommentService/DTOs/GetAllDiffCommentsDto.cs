using CommentService.Models;

namespace CommentService.DTOs;

public class GetAllDiffCommentsDto
{
    public List<Comment> GeneralDiffComments { get; set; } = new();
    public List<Comment> BeatNumberedComments { get; set; } = new();
}