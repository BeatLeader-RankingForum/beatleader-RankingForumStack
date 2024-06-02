using CommentService.Models;
using Contracts;
using Microsoft.EntityFrameworkCore;

namespace CommentService.Logic;

public class ReviewLogic
{
    private readonly CommentDbContext _dbContext;
    
    public ReviewLogic(CommentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<LogicResponse<List<Review>>> GetReviewsByMapDiscussionId(string mapDiscussionId)
    {
        List<Review> reviews = await _dbContext.Reviews.Where(r => r.MapDiscussionId == mapDiscussionId).ToListAsync();

        if (reviews.Count <= 0)
        {
            return LogicResponse<List<Review>>.Fail("No reviews found", LogicResponseType.NotFound);
        }
        
        return LogicResponse<List<Review>>.Ok(reviews);
    }
}