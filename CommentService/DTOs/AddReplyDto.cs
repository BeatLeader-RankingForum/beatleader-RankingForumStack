namespace CommentService.DTOs;

public class AddReplyDto
{
    public required string CommentId { get; set; }
    public required string Body { get; set; }
}