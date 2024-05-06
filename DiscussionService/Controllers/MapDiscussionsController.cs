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
using DiscussionService.Logic;

namespace DiscussionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MapDiscussionsController : ControllerBase
    {
        private readonly ILogger<MapDiscussionsController> _logger;
        private readonly AppDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly MapDiscussionLogic _mapDiscussionLogic;

        public MapDiscussionsController(ILogger<MapDiscussionsController> logger, AppDbContext context, IPublishEndpoint publishEndpoint, MapDiscussionLogic mapDiscussionLogic)
        {
            _logger = logger;
            _context = context;
            _publishEndpoint = publishEndpoint;
            _mapDiscussionLogic = mapDiscussionLogic;
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


        [HttpPost]
        public async Task<ActionResult<MapDiscussion>> PostMapDiscussion(CreateMapDiscussionDto mapDiscussionDto)
        {
            var result = await _mapDiscussionLogic.CreateMapDiscussion(mapDiscussionDto);

            if (result.Success)
            {
                if (result.Data == null)
                {
                    _logger.LogError("An error occurred while creating the map discussion. Result data was empty.");
                    return StatusCode(500, "An error occurred while creating the map discussion.");
                }
                return CreatedAtAction("GetMapDiscussion", new { id = result.Data.Id }, result.Data);
            }
            else
            {
                switch (result.Type)
                {
                    case LogicResponseType.Conflict:
                        return Conflict(result.ErrorMessage);
                    default:
                        return BadRequest(result.ErrorMessage);
                }
            }
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
