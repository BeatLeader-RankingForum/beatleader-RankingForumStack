using CommentService.DTOs;
using CommentService.Models;
using Contracts;
using Microsoft.EntityFrameworkCore;

namespace CommentService.Logic;

public class CommentLogic
{
    private readonly CommentDbContext _dbContext;
    
    public CommentLogic(CommentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<LogicResponse<Comment>> GetCommentByIdAsync(string id)
    {
        Comment? comment = await _dbContext.Comments
            .FirstOrDefaultAsync(c => c.Id == id);
        
        if (comment is null)
        {
            return LogicResponse<Comment>.Fail($"Comment with id {id} not found", LogicResponseType.NotFound);
        }
        
        return LogicResponse<Comment>.Ok(comment);
    }

    public async Task<LogicResponse<Comment>> AddCommentAsync(AddCommentDto commentDto, string userId)
    {
        // TODO: check if user exists
        
        
        if (await _dbContext.Comments.AnyAsync(
                u => u.AuthorId == userId && u.DifficultyDiscussionId == commentDto.DifficultyDiscussionId
                && u.MapDiscussionId == commentDto.MapDiscussionId && u.Type == commentDto.Type && u.Body == commentDto.Body))
        {
            return LogicResponse<Comment>.Fail("Duplicate comment", LogicResponseType.Conflict);
        }
        
        Comment comment = new()
        {
            AuthorId = userId,
            MapDiscussionId = commentDto.MapDiscussionId,
            DifficultyDiscussionId = commentDto.DifficultyDiscussionId,
            Body = commentDto.Body,
            ImageLink = commentDto.ImageLink,
            BeatNumber = commentDto.BeatNumber,
            Type = commentDto.Type,
            IsResolved = false,
            IsEdited = false
        };
        
        await _dbContext.Comments.AddAsync(comment);
        await _dbContext.SaveChangesAsync();
        
        return LogicResponse<Comment>.Ok(comment);
    }

    public async Task<LogicResponse<Comment>> EditCommentAsync(EditCommentDto commentDto, string userId)
    {
        Comment? comment = await _dbContext.Comments.FindAsync(commentDto.Id);

        if (comment is null || comment.IsDeleted)
        {
            return LogicResponse<Comment>.Fail($"Comment with id {commentDto.Id} not found", LogicResponseType.NotFound);
        }
        
        if (comment.AuthorId != userId)
        {
            return LogicResponse<Comment>.Fail("You are not the author of this comment", LogicResponseType.Forbidden);
        }

        if (commentDto.Body == null && commentDto.ImageLink == null && commentDto.BeatNumber == null
            && commentDto.Type == null)
        {
            return LogicResponse<Comment>.Fail("No values to update", LogicResponseType.BadRequest);
        }
        
        // update comment for values that arent null
        comment.Body = commentDto.Body ?? comment.Body;
        comment.ImageLink = commentDto.ImageLink ?? comment.ImageLink;
        comment.BeatNumber = commentDto.BeatNumber ?? comment.BeatNumber;
        comment.Type = commentDto.Type ?? comment.Type;
        comment.EditedAt = DateTime.UtcNow;
        comment.IsEdited = true;
        
        await _dbContext.SaveChangesAsync();
        
        return LogicResponse<Comment>.Ok(comment);
    }

    public async Task<LogicResponse<bool>> DeleteCommentAsync(string id, string userId, bool isModerator)
    {
        Comment? comment = await _dbContext.Comments.Include(c => c.Replies).FirstOrDefaultAsync(c => c.Id == id);

        if (comment is null || comment.IsDeleted)
        {
            return LogicResponse<bool>.Fail($"Comment with id {id} not found", LogicResponseType.NotFound);
        }
        
        if (comment.AuthorId != userId && !isModerator)
        {
            return LogicResponse<bool>.Fail("You are not the author of this comment", LogicResponseType.Forbidden);
        }

        if (comment.Replies.Count > 0)
        {
            comment.AuthorId = "anonymized";
            comment.Body = "";
            comment.ImageLink = null;
            comment.IsDeleted = true;
            comment.DeletedAt = DateTime.UtcNow;

            _dbContext.Comments.Update(comment);
            await _dbContext.SaveChangesAsync();
            return LogicResponse<bool>.Ok(true);
        }
        else
        {
            _dbContext.Comments.Remove(comment);
            await _dbContext.SaveChangesAsync();
            return LogicResponse<bool>.Ok(true);
        }
    }

    public async Task<LogicResponse<bool>> ResolveCommentAsync(string id, string userId)
    {
        Comment? comment = await _dbContext.Comments.Include(c => c.Replies).FirstOrDefaultAsync(c => c.Id == id);

        if (comment is null || comment.IsDeleted)
        {
            return LogicResponse<bool>.Fail($"Comment with id {id} not found", LogicResponseType.NotFound);
        }
        
        /* TODO: add discussion owner fetch and check
        if ([id of a discusion owner] != userId)
        {
            return LogicResponse<bool>.Fail("You are not a discussion owner", LogicResponseType.Forbidden);
        }*/
        
        if (comment.IsResolved)
        {
            return LogicResponse<bool>.Fail("Comment is already resolved", LogicResponseType.Conflict);
        }
        
        if (comment.Replies.Count <= 0)
        {
            return LogicResponse<bool>.Fail("Cannot resolve comment with no replies", LogicResponseType.Forbidden);
        }

        OrderedThreadItem resolvedEntry = CreateResolvedEntry(comment.Id, userId);
        
        comment.Replies.Add(resolvedEntry);
        comment.ResolvedAt = DateTime.UtcNow;
        comment.IsResolved = true;
        _dbContext.Comments.Update(comment);
        await _dbContext.SaveChangesAsync();
        return LogicResponse<bool>.Ok(true);
    }
    
    private OrderedThreadItem CreateResolvedEntry(string commentId, string userId)
    {
        return new StatusUpdate()
        {
            CommentId = commentId,
            AuthorId = userId,
            Type = UpdateType.Resolved
        };
    }
    
    public async Task<LogicResponse<bool>> ReopenCommentAsync(string id, string userId)
    {
        Comment? comment = await _dbContext.Comments.Include(c => c.Replies).FirstOrDefaultAsync(c => c.Id == id);

        if (comment is null || comment.IsDeleted)
        {
            return LogicResponse<bool>.Fail($"Comment with id {id} not found", LogicResponseType.NotFound);
        }
        
        if (comment.AuthorId != userId)
        {
            return LogicResponse<bool>.Fail("You are not the author of this comment", LogicResponseType.Forbidden);
        }
        
        if (!comment.IsResolved)
        {
            return LogicResponse<bool>.Fail("Comment is already unresolved", LogicResponseType.Conflict);
        }

        OrderedThreadItem reopenedEntry = CreateReopenedEntry(comment.Id, userId);
        
        comment.Replies.Add(reopenedEntry);
        comment.ResolvedAt = DateTime.UtcNow;
        comment.IsResolved = false;
        _dbContext.Comments.Update(comment);
        await _dbContext.SaveChangesAsync();
        return LogicResponse<bool>.Ok(true);
    }
    
    private OrderedThreadItem CreateReopenedEntry(string commentId, string userId)
    {
        return new StatusUpdate()
        {
            CommentId = commentId,
            AuthorId = userId,
            Type = UpdateType.Reopened
        };
    }
}