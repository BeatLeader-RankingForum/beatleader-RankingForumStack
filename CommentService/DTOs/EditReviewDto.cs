namespace CommentService.DTOs;

public class EditReviewDto
{
    public required string Id { get; set; }
    public string? Body { get; set; }
    public List<string>? CommentIds { get; set; }
}