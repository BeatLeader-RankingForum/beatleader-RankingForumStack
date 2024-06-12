using CommentService.DTOs;
using CommentService.Models;
using Contracts;
using Microsoft.EntityFrameworkCore;

namespace CommentService.Logic;

public class CommentLogic
{
    private readonly CommentDbContext _dbContext;
    private readonly ReviewLogic _reviewLogic;
    
    public CommentLogic(CommentDbContext dbContext, ReviewLogic reviewLogic)
    {
        _dbContext = dbContext;
        _reviewLogic = reviewLogic;
    }

    public async Task<LogicResponse<Comment>> GetCommentByIdAsync(string id)
    {
        Comment? comment = await _dbContext.Comments
            .Include(c => c.Replies)
            .Include(c => c.Events)
            .FirstOrDefaultAsync(c => c.Id == id);
        
        if (comment is null)
        {
            return LogicResponse<Comment>.Fail($"Comment with id {id} not found", LogicResponseType.NotFound);
        }
        
        return LogicResponse<Comment>.Ok(comment);
    }

    public async Task<LogicResponse<Comment>> AddCommentAsync(AddCommentDto commentDto, string userId)
    {
        // TODO; check if discussions exist
        
        if (await _dbContext.Comments.AnyAsync(
                u => u.AuthorId == userId && u.DifficultyDiscussionId == commentDto.DifficultyDiscussionId
                && u.MapDiscussionId == commentDto.MapDiscussionId && u.Type == commentDto.Type && u.Body == commentDto.Body))
        {
            return LogicResponse<Comment>.Fail("Duplicate comment", LogicResponseType.Conflict);
        }
        
        // TODO: stop comments with a beat number but no diffdiscussionid from being posted as such
        
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

        _dbContext.Comments.Update(comment);
        await _dbContext.SaveChangesAsync();
        
        return LogicResponse<Comment>.Ok(comment);
    }

    public async Task<LogicResponse<bool>> DeleteCommentAsync(string id, string userId, bool isModerator)
    {
        Comment? comment = await _dbContext.Comments.Include(c => c.Replies).FirstOrDefaultAsync(c => c.Id == id);

        if (comment is null || comment.IsDeleted)
        {
            return LogicResponse<bool>.Fail($"Comment with id {id} not found or already marked as deleted", LogicResponseType.NotFound);
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

        if (comment.Type == CommentType.Note || comment.Type == CommentType.Praise || comment.Type == CommentType.Hype)
        {
            return LogicResponse<bool>.Fail("Cannot resolve note, praise, or hype comment",
                LogicResponseType.Forbidden);
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

        StatusUpdate resolvedEntry = CreateResolvedEntry(comment.Id, userId);
        
        comment.Events.Add(resolvedEntry);
        comment.ResolvedAt = DateTime.UtcNow;
        comment.IsResolved = true;
        _dbContext.Comments.Update(comment);
        await _dbContext.SaveChangesAsync();
        return LogicResponse<bool>.Ok(true);
    }
    
    private StatusUpdate CreateResolvedEntry(string commentId, string userId)
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

        StatusUpdate reopenedEntry = CreateReopenedEntry(comment.Id, userId);
        
        comment.Events.Add(reopenedEntry);
        comment.ResolvedAt = DateTime.UtcNow;
        comment.IsResolved = false;
        _dbContext.Comments.Update(comment);
        await _dbContext.SaveChangesAsync();
        return LogicResponse<bool>.Ok(true);
    }
    
    private StatusUpdate CreateReopenedEntry(string commentId, string userId)
    {
        return new StatusUpdate()
        {
            CommentId = commentId,
            AuthorId = userId,
            Type = UpdateType.Reopened
        };
    }

    public async Task<LogicResponse<CommentStatsDto>> GetCommentStatsAsync(string mapDiscussionId, string difficultyDiscussionId, string? userId = null)
    {
        // TODO: check performance of this, maybe add caching or indexing
        int? userCommentsCount = null;
        if (userId is not null)
        {
            userCommentsCount = await _dbContext.Comments
                .Where(c => c.MapDiscussionId == mapDiscussionId
                            && (c.DifficultyDiscussionId == null || c.DifficultyDiscussionId == difficultyDiscussionId)
                            && c.IsDeleted == false && c.AuthorId == userId)
                .CountAsync();
        }
        
        int notesCount = await _dbContext.Comments
            .Where(c => c.MapDiscussionId == mapDiscussionId
                        && (c.DifficultyDiscussionId == null || c.DifficultyDiscussionId == difficultyDiscussionId)
                        && c.IsDeleted == false && c.Type == CommentType.Note)
            .CountAsync();
        
        int resolvedCount = await _dbContext.Comments
            .Where(c => c.MapDiscussionId == mapDiscussionId
                        && (c.DifficultyDiscussionId == null || c.DifficultyDiscussionId == difficultyDiscussionId)
                        && c.IsDeleted == false && c.IsResolved == true)
            .CountAsync();
        
        int pendingCount = await _dbContext.Comments
            .Where(c => c.MapDiscussionId == mapDiscussionId
                        && (c.DifficultyDiscussionId == null || c.DifficultyDiscussionId == difficultyDiscussionId)
                        && c.IsDeleted == false && c.IsResolved == false && c.Type != CommentType.Note && c.Type != CommentType.Praise && c.Type != CommentType.Hype)
            .CountAsync();
        
        int praiseCount = await _dbContext.Comments
            .Where(c => c.MapDiscussionId == mapDiscussionId
                        && (c.DifficultyDiscussionId == null || c.DifficultyDiscussionId == difficultyDiscussionId)
                        && c.IsDeleted == false && ( c.Type == CommentType.Praise || c.Type == CommentType.Hype))
            .CountAsync();
        
        int allCount = await _dbContext.Comments
            .Where(c => c.MapDiscussionId == mapDiscussionId
                        && (c.DifficultyDiscussionId == null || c.DifficultyDiscussionId == difficultyDiscussionId)
                        && c.IsDeleted == false)
            .CountAsync();
        
        int reviewCount = await _dbContext.Reviews.Where(c => c.MapDiscussionId == mapDiscussionId && c.IsDeleted == false).CountAsync();
        allCount += reviewCount;
        
        return LogicResponse<CommentStatsDto>.Ok(new CommentStatsDto
        {
            UserCommentsCount = userCommentsCount ?? 0,
            AllCommentsCount = allCount,
            NotesCount = notesCount,
            ResolvedCount = resolvedCount,
            OpenCount = pendingCount,
            PraiseCount = praiseCount,
        });
    }

    public async Task<LogicResponse<GetAllDiffCommentsDto>> GetAllDifficultyCommentsAsync(string difficultyDiscussionId)
    {
        // TODO: change this to not give not found if there are no comments, but only if the difficultydiscussionid doesnt exist?
        List<Comment> comments = await _dbContext.Comments
            .Where(c => c.DifficultyDiscussionId == difficultyDiscussionId)
            .Include(c => c.Replies)
            .Include(c => c.Events)
            .ToListAsync();
        
        if (comments.Count == 0)
        {
            return LogicResponse<GetAllDiffCommentsDto>.Fail("No comments found for given difficulty discussion", LogicResponseType.NotFound);
        }
        
        GetAllDiffCommentsDto commentsDto = new GetAllDiffCommentsDto
        {
            GeneralDiffComments = comments.Where(c => c.BeatNumber == null).ToList(),
            BeatNumberedComments = comments.Where(c => c.BeatNumber != null).ToList()
        };
        
        return LogicResponse<GetAllDiffCommentsDto>.Ok(commentsDto);
    }

    public async Task<LogicResponse<GetAllMapCommentsDto>> GetAllMapsetCommentsAsync(string mapDiscussionId)
    {
        List<Comment> comments = await _dbContext.Comments
            .Where(c => c.MapDiscussionId == mapDiscussionId && c.DifficultyDiscussionId == null)
            .Include(c => c.Replies)
            .Include(c => c.Events)
            .ToListAsync();
        
        if (comments.Count == 0)
        {
            return LogicResponse<GetAllMapCommentsDto>.Fail("No comments found for given mapset discussion", LogicResponseType.NotFound);
        }

        var reviewsResult = await _reviewLogic.GetReviewsByMapDiscussionIdAsync(mapDiscussionId);
        if (reviewsResult.Success == false && reviewsResult.Type != LogicResponseType.NotFound)
        {
            return LogicResponse<GetAllMapCommentsDto>.Fail(reviewsResult.ErrorMessage!, reviewsResult.Type);
        }
        List<Review> reviews = reviewsResult.Data ?? new List<Review>();
        
        GetAllMapCommentsDto commentsDto = new GetAllMapCommentsDto
        {
            GeneralMapComments = comments,
            MapReviews = reviews
        };
        
        return LogicResponse<GetAllMapCommentsDto>.Ok(commentsDto);
    }

    public async Task LoadtestCleanup()
    {
        if (Environment.GetEnvironmentVariable("LOADTEST") != "true")
        {
            return;
        }
        
        List<Comment> comments = await _dbContext.Comments
            .Where(c => c.Body.Contains("LOADTEST-54632"))
            .ToListAsync();
        
        _dbContext.RemoveRange(comments);
        
        await _dbContext.SaveChangesAsync();
    }
}