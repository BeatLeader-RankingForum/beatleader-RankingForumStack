using CommentService.Models;

namespace CommentService.DTOs;

public class GetAllMapCommentsDto
{
    public List<Comment> GeneralMapComments { get; set; } = new();
    public List<Review> MapReviews { get; set; } = new();
}