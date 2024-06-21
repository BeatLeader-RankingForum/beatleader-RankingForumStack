using Contracts;
using DiscussionService.DTOs;
using DiscussionService.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace DiscussionService.Logic
{
    public class MapDiscussionLogic
    {
        private readonly DiscussionDbContext _dbContext;
        private readonly IPublishEndpoint _publishEndpoint;

        public MapDiscussionLogic(DiscussionDbContext dbContext, IPublishEndpoint publishEndpoint)
        {
            _dbContext = dbContext;
            _publishEndpoint = publishEndpoint;
        }
        
        public async Task<LogicResponse<MapDiscussion>> GetMapDiscussionAsync(string id)
        {
            MapDiscussion? mapDiscussion = await _dbContext.MapDiscussions
                .Include(x => x.Discussions)
                .FirstOrDefaultAsync(x => x.Id == id);
            
            if (mapDiscussion == null)
            {
                return LogicResponse<MapDiscussion>.Fail($"MapDiscussion with id {id} not found", LogicResponseType.NotFound);
            }
            
            return LogicResponse<MapDiscussion>.Ok(mapDiscussion);
        }

        public async Task<LogicResponse<MapDiscussion>> AddMapDiscussionAsync(AddMapDiscussionDto mapDiscussionDto)
        {
            if (await _dbContext.MapDiscussions.AnyAsync(x => x.MapsetId == mapDiscussionDto.MapsetId))
            {
                return LogicResponse<MapDiscussion>.Fail($"An entry already exists for {mapDiscussionDto.MapsetId}", LogicResponseType.Conflict);
            }
            
            // TODO: logic to determine if a map discussion should be made (does the map exist)

            List<DifficultyDiscussion> discussions = new();
            // TODO: logic to determine which difficulty discussions to create

            discussions.Add(new DifficultyDiscussion() // TODO: REMOVE PLACEHOLDER!!
            {
                Characteristic = 1,
                Difficulty = 1
            });

            List<string> owners = new();
            // TODO: logic to determine owners

            owners.Add("76561198051924392"); // TODO: REMOVE PLACEHOLDER!!


            MapDiscussion mapDiscussion = new()
            {
                MapsetId = mapDiscussionDto.MapsetId,
                DiscussionOwnerIds = owners,
                Discussions = discussions
            };
            
            await _dbContext.MapDiscussions.AddAsync(mapDiscussion);
            await _dbContext.SaveChangesAsync();

            await _publishEndpoint.Publish(new DiscussionCreatedEvent()
            {
                Id = mapDiscussion.Id,
                MapsetId = mapDiscussion.MapsetId,
                CreatedOnUtc = mapDiscussion.CreatedOnUtc
            });

            return LogicResponse<MapDiscussion>.Ok(mapDiscussion);
        }
        
        public async Task<LogicResponse<List<MapDiscussion>>> GetAllMapDiscussionsForPhaseAsync(string phase)
        {
            if (Enum.TryParse(phase, out PhaseEnum phaseEnum))
            {
                return LogicResponse<List<MapDiscussion>>.Fail($"Phase {phase} not found", LogicResponseType.BadRequest);
            }

            List<MapDiscussion> mapDiscussions = await _dbContext.MapDiscussions.Where(x => x.Phase == phaseEnum).Include(x => x.Discussions).ToListAsync();

            if (mapDiscussions.Count <= 0)
            {
                return LogicResponse<List<MapDiscussion>>.Fail("No map discussions found", LogicResponseType.NotFound);
            }
            
            return LogicResponse<List<MapDiscussion>>.Ok(mapDiscussions);
        }

        public async Task<LogicResponse<MapDiscussion>> RequestOwnershipAsync(RequestOwnershipDto requestOwnershipDto, string userId)
        {
            MapDiscussion? mapDiscussion =
                await _dbContext.MapDiscussions.FindAsync(requestOwnershipDto.MapDiscussionId);
            
            if (mapDiscussion is null)
            {
                return LogicResponse<MapDiscussion>.Fail($"MapDiscussion with id {requestOwnershipDto.MapDiscussionId} not found", LogicResponseType.NotFound);
            }
            
            if (mapDiscussion.DiscussionOwnerIds.Contains(userId))
            {
                return LogicResponse<MapDiscussion>.Fail("User already owns this map discussion", LogicResponseType.Conflict);
            }
            
            // TODO: IMPORTANT: logic to determine if user is allowed to request ownership
            
            mapDiscussion.DiscussionOwnerIds.Add(userId);
            await _dbContext.SaveChangesAsync();
            
            return LogicResponse<MapDiscussion>.Ok(mapDiscussion);
        }
    }
}
