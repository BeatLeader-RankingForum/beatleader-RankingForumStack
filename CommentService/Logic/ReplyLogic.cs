using CommentService.DTOs;
using CommentService.Models;
using Contracts;
using Microsoft.EntityFrameworkCore;

namespace CommentService.Logic;

public class ReplyLogic
{
    private readonly CommentDbContext _dbContext;
    
    public ReplyLogic(CommentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<LogicResponse<List<Reply>>> GetAllRepliesToCommentAsync(string commentId)
    {
        if (!await _dbContext.Comments
                .AnyAsync(c => c.Id == commentId))
        {
            return LogicResponse<List<Reply>>.Fail("Comment not found", LogicResponseType.NotFound);
        }
        
        List<Reply> replies = await _dbContext.Replies.Where(r => r.CommentId == commentId).ToListAsync();
        
        return LogicResponse<List<Reply>>.Ok(replies);
    }

    public async Task<LogicResponse<Reply>> GetReplyByIdAsync(string id)
    {
        Reply? reply = await _dbContext.Replies.FindAsync(id);
        
        if (reply is null)
        {
            return LogicResponse<Reply>.Fail("Reply not found", LogicResponseType.NotFound);
        }
        
        return LogicResponse<Reply>.Ok(reply);
    }

    public async Task<LogicResponse<Reply>> AddReplyAsync(AddReplyDto reply, string userId)
    {
        if (!await _dbContext.Comments
                .AnyAsync(c => c.Id == reply.CommentId && !c.IsDeleted))
        {
            return LogicResponse<Reply>.Fail("Comment of reply not found or deleted", LogicResponseType.BadRequest);
        }
        
        Reply replyToAdd = new()
        {
            AuthorId = userId,
            CommentId = reply.CommentId,
            Body = reply.Body
        };

        if (await _dbContext.Replies.AnyAsync(
                r => r.AuthorId == replyToAdd.AuthorId
                     && r.CommentId == replyToAdd.CommentId && r.Body == replyToAdd.Body))
        {
            return LogicResponse<Reply>.Fail("Duplicate reply", LogicResponseType.Conflict);
        }
        
        await _dbContext.Replies.AddAsync(replyToAdd);
        await _dbContext.SaveChangesAsync();
        
        return LogicResponse<Reply>.Ok(replyToAdd);
    }
    
    // TODO: add review replying

    public async Task<LogicResponse<Reply>> EditReplyAsync(EditReplyDto replyDto, string userId)
    {
        Reply? reply = await _dbContext.Replies.FindAsync(replyDto.Id);

        if (reply is null || reply.IsDeleted)
        {
            return LogicResponse<Reply>.Fail("Reply not found or deleted", LogicResponseType.NotFound);
        }
        
        if (reply.AuthorId != userId)
        {
            return LogicResponse<Reply>.Fail("Unauthorized", LogicResponseType.Forbidden);
        }

        if (replyDto.Body == null)
        {
            return LogicResponse<Reply>.Fail("No values to update", LogicResponseType.BadRequest);
        }
        
        reply.Body = replyDto.Body ?? reply.Body;
        reply.EditedAt = DateTime.UtcNow;
        reply.IsEdited = true;
        
        _dbContext.Replies.Update(reply);
        await _dbContext.SaveChangesAsync();
        
        return LogicResponse<Reply>.Ok(reply);
    }

    public async Task<LogicResponse<bool>> DeleteReplyAsync(string id, string userId, bool isModerator)
    {
        Reply? reply = await _dbContext.Replies.FindAsync(id);

        if (reply is null || reply.IsDeleted)
        {
            return LogicResponse<bool>.Fail("Reply not found or deleted", LogicResponseType.NotFound);
        }
        
        if (reply.AuthorId != userId && !isModerator)
        {
            return LogicResponse<bool>.Fail("You are not the author of this comment", LogicResponseType.Forbidden);
        }

        reply.AuthorId = "anonymized";
        reply.Body = "";
        reply.IsDeleted = true;
        reply.DeletedAt = DateTime.UtcNow;
        
        _dbContext.Replies.Update(reply);
        await _dbContext.SaveChangesAsync();
        
        return LogicResponse<bool>.Ok(true);
    }
}