using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DiscussionService;
using DiscussionService.Models;
using DiscussionService.DTOs;
using MassTransit;
using Contracts;

namespace DiscussionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MapDiscussionsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public MapDiscussionsController(AppDbContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        // GET: api/MapDiscussions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MapDiscussion>>> GetMapDiscussions()
        {
            return await _context.MapDiscussions.AsNoTracking().Include(x => x.Discussions).ToListAsync();
        }

        // GET: api/MapDiscussions/by-id?mapDiscussionId=5
        [HttpGet("by-id")]
        public async Task<ActionResult<MapDiscussion>> GetMapDiscussion([FromQuery] int? mapDiscussionId, [FromQuery] string? mapsetId)
        {
            MapDiscussion? mapDiscussion;

            if (mapDiscussionId.HasValue)
            {
                mapDiscussion = await _context.MapDiscussions.Include(x => x.Discussions).FirstOrDefaultAsync(x => x.Id == mapDiscussionId);
            }
            else if (mapsetId != null)
            {
                mapDiscussion = await _context.MapDiscussions.Include(x => x.Discussions).FirstOrDefaultAsync(x => x.MapsetId == mapsetId);
            }
            else
            {
                return BadRequest("A valid ID must be provided.");
            }

            if (mapDiscussion == null)
            {
                return NotFound();
            }

            return mapDiscussion;


        }

        // PUT: api/MapDiscussions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMapDiscussion(int id, MapDiscussion mapDiscussion)
        {
            if (id != mapDiscussion.Id)
            {
                return BadRequest();
            }

            _context.Entry(mapDiscussion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await MapDiscussionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/MapDiscussions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MapDiscussion>> PostMapDiscussion(CreateMapDiscussionDto mapDiscussionDto)
        {
            if (await _context.MapDiscussions.AnyAsync(x => x.MapsetId == mapDiscussionDto.MapsetId))
            {
                return Conflict($"A MapDiscussion already exists for {mapDiscussionDto.MapsetId}");
            }

            List<Discussion> discussions = new();
            // TODO: logic to determine which discussions to create

            List<string> owners = new();
            // TODO: logic to determine owners


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

            return CreatedAtAction("GetMapDiscussion", new { id = mapDiscussion.Id }, mapDiscussion);
        }

        [HttpGet("{id}/ownership")]
        public async Task<ActionResult> ClaimOwnership(int id)
        {
            // TODO: implement this method
            // get current user from request

            // get map id from map discussion id

            // assess if user is allowed to claim ownership

            throw new NotImplementedException();
        }

        // DELETE: api/MapDiscussions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMapDiscussion(int id)
        {
            var mapDiscussion = await _context.MapDiscussions.FindAsync(id);
            if (mapDiscussion == null)
            {
                return NotFound();
            }

            _context.MapDiscussions.Remove(mapDiscussion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> MapDiscussionExists(int id)
        {
            return await _context.MapDiscussions.AnyAsync(e => e.Id == id);
        }
    }
}
