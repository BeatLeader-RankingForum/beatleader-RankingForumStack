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

namespace DiscussionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MapDiscussionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MapDiscussionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/MapDiscussions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MapDiscussion>>> GetMapDiscussions()
        {
            return await _context.MapDiscussions.AsNoTracking().Include(x => x.Discussions).ToListAsync();
        }

        // GET: api/MapDiscussions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MapDiscussion>> GetMapDiscussion(int id)
        {
            var mapDiscussion = await _context.MapDiscussions.FindAsync(id);

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
