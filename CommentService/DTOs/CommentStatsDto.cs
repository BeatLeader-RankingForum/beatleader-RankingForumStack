namespace CommentService.DTOs;

public class CommentStatsDto
{
    public int UserCommentsCount { get; set; }
    public int AllCommentsCount { get; set; }
    public int NotesCount { get; set; }
    public int ResolvedCount { get; set; }
    public int OpenCount { get; set; }
    public int PraiseCount { get; set; }
}