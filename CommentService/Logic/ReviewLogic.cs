using CommentService.DTOs;
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

    public async Task<LogicResponse<List<Review>>> GetReviewsByMapDiscussionIdAsync(string mapDiscussionId)
    {
        List<Review> reviews = await _dbContext.Reviews.Where(r => r.MapDiscussionId == mapDiscussionId).ToListAsync();

        if (reviews.Count <= 0)
        {
            return LogicResponse<List<Review>>.Fail("No reviews found", LogicResponseType.NotFound);
        }
        
        return LogicResponse<List<Review>>.Ok(reviews);
    }

    public async Task<LogicResponse<Review>> GetReviewByIdAsync(string id)
    {
        Review? review = await _dbContext.Reviews.FindAsync(id);
        
        if (review is null)
        {
            return LogicResponse<Review>.Fail("Review not found", LogicResponseType.NotFound);
        }
        
        return LogicResponse<Review>.Ok(review);
    }

    public async Task<LogicResponse<Review>> AddReviewAsync(AddReviewDto reviewDto, string userId)
    {
        if (reviewDto.CommentIds.Count <= 0)
        {
            return LogicResponse<Review>.Fail("No comments provided", LogicResponseType.BadRequest);
        }
        
        // check all comments
        var commentCheck = await CheckIfCommentsAreAllowed(reviewDto.CommentIds, reviewDto.MapDiscussionId, userId);
        if (!commentCheck.Success)
        {
            return LogicResponse<Review>.Fail(commentCheck.ErrorMessage!, commentCheck.Type);
        }

        Review review = new()
        {
            AuthorId = userId,
            MapDiscussionId = reviewDto.MapDiscussionId,
            CommentIds = reviewDto.CommentIds,
            Body = reviewDto.Body,
            IsEdited = false,
            CreatedAt = DateTime.UtcNow
        };

        if (await _dbContext.Reviews.AnyAsync(
                r => r.AuthorId == review.AuthorId && r.MapDiscussionId == review.MapDiscussionId
                                                   && r.CommentIds.Equals(review.CommentIds)))
        {
            return LogicResponse<Review>.Fail("Duplicate review", LogicResponseType.Conflict);
        }

        await _dbContext.Reviews.AddAsync(review);
        await _dbContext.SaveChangesAsync();
        
        return LogicResponse<Review>.Ok(review);
    }

    public async Task<LogicResponse<Review>> EditReviewAsync(EditReviewDto reviewDto, string userId)
    {
        Review? review = await _dbContext.Reviews.FindAsync(reviewDto.Id);
        
        if (review is null || review.IsDeleted)
        {
            return LogicResponse<Review>.Fail($"Review with id {reviewDto.Id} not found", LogicResponseType.NotFound);
        }
        
        if (review.AuthorId != userId)
        {
            return LogicResponse<Review>.Fail("You are not the author of this review", LogicResponseType.Forbidden);
        }
        
        if (reviewDto.Body == null && reviewDto.CommentIds == null)
        {
            return LogicResponse<Review>.Fail("No values to update", LogicResponseType.BadRequest);
        }

        if (reviewDto.CommentIds is not null)
        {
            var commentCheck = await CheckIfCommentsAreAllowed(reviewDto.CommentIds, review.MapDiscussionId, userId);
            if (!commentCheck.Success)
            {
                return LogicResponse<Review>.Fail(commentCheck.ErrorMessage!, commentCheck.Type);
            }
        }
        
        
        // update review for values that arent null
        review.Body = reviewDto.Body ?? review.Body;
        review.CommentIds = reviewDto.CommentIds ?? review.CommentIds;
        review.EditedAt = DateTime.UtcNow;
        review.IsEdited = true;
        
        await _dbContext.SaveChangesAsync();
        
        return LogicResponse<Review>.Ok(review);
    }
    
    public async Task<LogicResponse<bool>> DeleteReviewAsync(string id, string userId, bool isModerator)
    {
        Review? review = await _dbContext.Reviews.Include(r => r.Replies).FirstOrDefaultAsync(r => r.Id == id);
        
        if (review is null || review.IsDeleted)
        {
            return LogicResponse<bool>.Fail($"Review with id {id} not found", LogicResponseType.NotFound);
        }
        
        if (review.AuthorId != userId && !isModerator)
        {
            return LogicResponse<bool>.Fail("You are not the author of this review", LogicResponseType.Forbidden);
        }

        if (review.Replies.Count > 0)
        {
            review.AuthorId = "anonymized";
            review.Body = "";
            review.CommentIds = new List<string>();
            review.IsDeleted = true;
            review.DeletedAt = DateTime.UtcNow;
            
            _dbContext.Reviews.Update(review);
            await _dbContext.SaveChangesAsync();
            return LogicResponse<bool>.Ok(true);
        }
        else
        {
            _dbContext.Reviews.Remove(review);
            await _dbContext.SaveChangesAsync();
            return LogicResponse<bool>.Ok(true);
        }
    }

    private async Task<LogicResponse<bool>> CheckIfCommentsAreAllowed(List<string> commentIds, string mapDiscussionId, string userId)
    {
        foreach (var commentId in commentIds)
        {
            Comment? comment = await _dbContext.Comments.FindAsync(commentId);
            if (comment is null || comment.IsDeleted)
            {
                return LogicResponse<bool>.Fail($"Comment {commentId} not found or is deleted", LogicResponseType.BadRequest);
            }

            if (comment.AuthorId != userId)
            {
                return LogicResponse<bool>.Fail($"Comment {commentId} is not from user {userId}", LogicResponseType.Forbidden);
            }

            if (comment.MapDiscussionId != mapDiscussionId)
            {
                return LogicResponse<bool>.Fail(
                    $"Comment {commentId} is not from map discussion {mapDiscussionId}",
                    LogicResponseType.BadRequest);
            }
        }

        return LogicResponse<bool>.Ok(true);
    }
}