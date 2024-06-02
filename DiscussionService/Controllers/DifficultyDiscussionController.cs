using Contracts;
using Contracts.Auth;
using DiscussionService.Logic;
using DiscussionService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DiscussionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DifficultyDiscussionController : ControllerBase
    {
        private readonly DifficultyDiscussionLogic _difficultyDiscussionLogic;
        
        public DifficultyDiscussionController(DifficultyDiscussionLogic difficultyDiscussionLogic)
        {
            _difficultyDiscussionLogic = difficultyDiscussionLogic;
        }
        
        // get all difficulty discussions by map discussion id
        [HttpGet("all/{mapDiscussionId}")]
        public async Task<ActionResult<List<DifficultyDiscussion>>> GetAllDifficultyDiscussionsForMapDiscussion(int mapDiscussionId)
        {
            LogicResponse<List<DifficultyDiscussion>> response = await _difficultyDiscussionLogic.GetAllDifficultyDiscussionsForMapDiscussionAsync(mapDiscussionId);
            
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
                    throw new Exception($"Method GetAllDifficultyDiscussionsForMapDiscussionAsync at GetAllDifficultyDiscussionsForMapDiscussion(id) returned an unexpected response type. Message: {response.ErrorMessage}");
            }
            
            return Ok(response.Data);
        }
        
        
        // get difficulty discussion by id
        [HttpGet("{id}")]
        public async Task<ActionResult<DifficultyDiscussion>> GetDifficultyDiscussion(int id)
        {
            LogicResponse<DifficultyDiscussion> response = await _difficultyDiscussionLogic.GetDifficultyDiscussionAsync(id);
            
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
                    throw new Exception($"Method GetDifficultyDiscussionAsync at GetDifficultyDiscussion(id) returned an unexpected response type. Message: {response.ErrorMessage}");
            }
            
            return Ok(response.Data);
        }
        
        // lock difficulty discussion
        [Authorize(Roles = nameof(Role.Moderator))]
        [HttpPost("{id}/lock")]
        public async Task<ActionResult<DifficultyDiscussion>> LockDifficultyDiscussion(int id)
        {
            LogicResponse<DifficultyDiscussion> response = await _difficultyDiscussionLogic.LockDifficultyDiscussionAsync(id);
            
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
                    throw new Exception($"Method LockDifficultyDiscussionAsync at LockDifficultyDiscussion(id) returned an unexpected response type. Message: {response.ErrorMessage}");
            }
            
            return Ok(response.Data);
        }
        
        // unlock difficulty discussion
        [Authorize(Roles = nameof(Role.Moderator))]
        [HttpPost("{id}/unlock")]
        public async Task<ActionResult<DifficultyDiscussion>> UnlockDifficultyDiscussion(int id)
        {
            LogicResponse<DifficultyDiscussion> response = await _difficultyDiscussionLogic.UnlockDifficultyDiscussionAsync(id);
            
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
                    throw new Exception($"Method UnlockDifficultyDiscussionAsync at UnlockDifficultyDiscussion(id) returned an unexpected response type. Message: {response.ErrorMessage}");
            }
            
            return Ok(response.Data);
        }
        
    }
}
