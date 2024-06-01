using Contracts;
using DiscussionService.DTOs;
using DiscussionService.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace DiscussionService.Logic
{
    public class MapDiscussionLogic
    {
        private readonly DiscussionDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public MapDiscussionLogic(DiscussionDbContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<LogicResponse<MapDiscussion>> CreateMapDiscussion(CreateMapDiscussionDto mapDiscussionDto)
        {
            if (await _context.MapDiscussions.AnyAsync(x => x.MapsetId == mapDiscussionDto.MapsetId))
            {
                return LogicResponse<MapDiscussion>.Fail($"An entry already exists for {mapDiscussionDto.MapsetId}", LogicResponseType.Conflict);
            }

            List<DifficultyDiscussion> discussions = new();
            // TODO: logic to determine which discussions to create

            discussions.Add(new DifficultyDiscussion() // placeholder
            {
                Characteristic = 1,
                Difficulty = 1
            });

            List<string> owners = new();
            // TODO: logic to determine owners

            owners.Add("76561198051924392"); // placeholder


            MapDiscussion mapDiscussion = new()
            {
                MapsetId = mapDiscussionDto.MapsetId,
                DiscussionOwnerIds = owners,
                Discussions = discussions
            };

            _context.MapDiscussions.Add(mapDiscussion);
            await _context.SaveChangesAsync();

            await _publishEndpoint.Publish(new DiscussionCreatedEvent()
            {
                Id = mapDiscussion.Id,
                MapsetId = mapDiscussion.MapsetId,
                CreatedOnUtc = mapDiscussion.CreatedOnUtc
            });

            return LogicResponse<MapDiscussion>.Ok(mapDiscussion);
        }
    }
}
