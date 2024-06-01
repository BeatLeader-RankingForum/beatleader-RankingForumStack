namespace CommentService.Models;

public class Reply : OrderedThreadItem
{
    public required string Body { get; set; }
}