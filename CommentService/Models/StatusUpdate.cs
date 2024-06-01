namespace CommentService.Models;

public class StatusUpdate : OrderedThreadItem
{
    public UpdateType Type { get; set; }
}

public enum UpdateType
{
    Resolved = 0,
    Reopened = 1
}