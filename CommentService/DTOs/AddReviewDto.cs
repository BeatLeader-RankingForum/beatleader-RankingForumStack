namespace CommentService.DTOs;

public class AddReviewDto
{
    public required string MapDiscussionId { get; set; }
    
    public required string Body { get; set; }

    public List<string> CommentIds { get; set; } = new();
}