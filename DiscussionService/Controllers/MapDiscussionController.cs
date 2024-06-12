using Contracts;
using Contracts.Auth;
using DiscussionService.DTOs;
using DiscussionService.Logic;
using DiscussionService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DiscussionService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MapDiscussionController : ControllerBase
    {
        private readonly MapDiscussionLogic _mapDiscussionLogic;

        public MapDiscussionController(MapDiscussionLogic mapDiscussion)
        {
            _mapDiscussionLogic = mapDiscussion;
        }

        // get map discussion by id
        [HttpGet("{id}")]
        public async Task<ActionResult<MapDiscussion>> GetMapDiscussion(string id)
        {
            LogicResponse<MapDiscussion> response = await _mapDiscussionLogic.GetMapDiscussionAsync(id);
            
            switch (response.Type)
            {
                case LogicResponseType.None:
                    break;
                case LogicResponseType.NotFound:
                    return NotFound(response.ErrorMessage);
                case LogicResponseType.Conflict:
                    return Conflict(response.ErrorMessage);
                case LogicResponseType.BadRequest:
                    return BadRequest(response.ErrorMessage);
                case LogicResponseType.Unauthorized:
                    return Unauthorized(response.ErrorMessage);
                case LogicResponseType.Forbidden:
                    return Forbid(response.ErrorMessage!);
                default:
                    throw new Exception($"Method GetMapDiscussionAsync at GetMapDiscussion(id) returned an unexpected response type. Message: {response.ErrorMessage}");
            }
            
            return Ok(response.Data);
        }
        
        // get all map discussions in specified phase
        [HttpGet("all/{phase}")]
        public async Task<ActionResult<List<MapDiscussion>>> GetAllMapDiscussionsForPhase(string phase)
        {
            LogicResponse<List<MapDiscussion>> response = await _mapDiscussionLogic.GetAllMapDiscussionsForPhaseAsync(phase);
            
            switch (response.Type)
            {
                case LogicResponseType.None:
                    break;
                case LogicResponseType.NotFound:
                    return NotFound(response.ErrorMessage);
                case LogicResponseType.Conflict:
                    return Conflict(response.ErrorMessage);
                case LogicResponseType.BadRequest:
                    return BadRequest(response.ErrorMessage);
                case LogicResponseType.Unauthorized:
                    return Unauthorized(response.ErrorMessage);
                case LogicResponseType.Forbidden:
                    return Forbid(response.ErrorMessage!);
                default:
                    throw new Exception($"Method GetAllMapDiscussionsForPhaseAsync at GetAllMapDiscussionsForPhase(phase) returned an unexpected response type. Message: {response.ErrorMessage}");
            }
            
            return Ok(response.Data);
        }
        
        // add map discussion
        [Authorize(Roles = nameof(Role.User))]
        [HttpPost]
        public async Task<ActionResult<MapDiscussion>> AddMapDiscussion(AddMapDiscussionDto discussionDto)
        {
            LogicResponse<MapDiscussion> response = await _mapDiscussionLogic.AddMapDiscussionAsync(discussionDto);
            
            switch (response.Type)
            {
                case LogicResponseType.None:
                    break;
                case LogicResponseType.NotFound:
                    return NotFound(response.ErrorMessage);
                case LogicResponseType.Conflict:
                    return Conflict(response.ErrorMessage);
                case LogicResponseType.BadRequest:
                    return BadRequest(response.ErrorMessage);
                case LogicResponseType.Unauthorized:
                    return Unauthorized(response.ErrorMessage);
                case LogicResponseType.Forbidden:
                    return Forbid(response.ErrorMessage!);
                default:
                    throw new Exception($"Method AddMapDiscussionAsync at AddMapDiscussion(discussionDto) returned an unexpected response type. Message: {response.ErrorMessage}");
            }
            
            return Ok(response.Data);
        }
        
        // edit map discussion owners (request access)
        [Authorize(Roles = nameof(Role.User))]
        [HttpPost("request-ownership")]
        public async Task<ActionResult<MapDiscussion>> RequestOwnership(RequestOwnershipDto requestOwnershipDto)
        {
            string? userId = User.Identity?.Name;
            
            if (userId is null)
            {
                return Forbid();
            }
            
            LogicResponse<MapDiscussion> response = await _mapDiscussionLogic.RequestOwnershipAsync(requestOwnershipDto, userId);
            
            switch (response.Type)
            {
                case LogicResponseType.None:
                    break;
                case LogicResponseType.NotFound:
                    return NotFound(response.ErrorMessage);
                case LogicResponseType.Conflict:
                    return Conflict(response.ErrorMessage);
                case LogicResponseType.BadRequest:
                    return BadRequest(response.ErrorMessage);
                case LogicResponseType.Unauthorized:
                    return Unauthorized(response.ErrorMessage);
                case LogicResponseType.Forbidden:
                    return Forbid(response.ErrorMessage!);
                default:
                    throw new Exception($"Method RequestOwnershipAsync at RequestOwnership(requestOwnershipDto) returned an unexpected response type. Message: {response.ErrorMessage}");
            }
            
            return Ok(response.Data);
        }
        
    }
}
