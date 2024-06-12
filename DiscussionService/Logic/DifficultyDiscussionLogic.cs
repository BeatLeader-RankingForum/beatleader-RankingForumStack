using Contracts;
using DiscussionService.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscussionService.Logic;

public class DifficultyDiscussionLogic
{
    private readonly DiscussionDbContext _dbContext;

    public DifficultyDiscussionLogic(DiscussionDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<LogicResponse<DifficultyDiscussion>> GetDifficultyDiscussionAsync(string id)
    {
        DifficultyDiscussion? difficultyDiscussion = await _dbContext.DifficultyDiscussions
            .FirstOrDefaultAsync(x => x.Id == id);
        
        if (difficultyDiscussion is null)
        {
            return LogicResponse<DifficultyDiscussion>.Fail($"DifficultyDiscussion with id {id} not found", LogicResponseType.NotFound);
        }
        
        return LogicResponse<DifficultyDiscussion>.Ok(difficultyDiscussion);
    }

    public async Task<LogicResponse<List<DifficultyDiscussion>>> GetAllDifficultyDiscussionsForMapDiscussionAsync(string mapDiscussionId)
    {
        List<DifficultyDiscussion> difficultyDiscussions = await _dbContext.DifficultyDiscussions
            .Where(x => x.MapDiscussionId == mapDiscussionId)
            .ToListAsync();
        
        if (difficultyDiscussions.Count == 0)
        {
            return LogicResponse<List<DifficultyDiscussion>>.Fail($"No difficulty discussions found for map discussion with id {mapDiscussionId}", LogicResponseType.NotFound);
        }
        
        return LogicResponse<List<DifficultyDiscussion>>.Ok(difficultyDiscussions);
    }

    public async Task<LogicResponse<DifficultyDiscussion>> LockDifficultyDiscussionAsync(string id)
    {
        DifficultyDiscussion? difficultyDiscussion = await _dbContext.DifficultyDiscussions
            .FirstOrDefaultAsync(x => x.Id == id);
        
        if (difficultyDiscussion is null)
        {
            return LogicResponse<DifficultyDiscussion>.Fail($"DifficultyDiscussion with id {id} not found", LogicResponseType.NotFound);
        }
        
        if (difficultyDiscussion.IsLocked)
        {
            return LogicResponse<DifficultyDiscussion>.Fail($"DifficultyDiscussion with id {id} is already locked", LogicResponseType.BadRequest);
        }
        
        difficultyDiscussion.IsLocked = true;
        
        _dbContext.DifficultyDiscussions.Update(difficultyDiscussion);
        await _dbContext.SaveChangesAsync();
        
        return LogicResponse<DifficultyDiscussion>.Ok(difficultyDiscussion);
    }
    
    public async Task<LogicResponse<DifficultyDiscussion>> UnlockDifficultyDiscussionAsync(string id)
    {
        DifficultyDiscussion? difficultyDiscussion = await _dbContext.DifficultyDiscussions
            .FirstOrDefaultAsync(x => x.Id == id);
        
        if (difficultyDiscussion is null)
        {
            return LogicResponse<DifficultyDiscussion>.Fail($"DifficultyDiscussion with id {id} not found", LogicResponseType.NotFound);
        }

        if (!difficultyDiscussion.IsLocked)
        {
            return LogicResponse<DifficultyDiscussion>.Fail($"DifficultyDiscussion with id {id} is already unlocked", LogicResponseType.BadRequest);
        }
        
        difficultyDiscussion.IsLocked = false;
        
        _dbContext.DifficultyDiscussions.Update(difficultyDiscussion);
        await _dbContext.SaveChangesAsync();
        
        return LogicResponse<DifficultyDiscussion>.Ok(difficultyDiscussion);
    }
}