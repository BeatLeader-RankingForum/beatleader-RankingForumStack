using System.Security.Claims;
using CommentService.DTOs;
using CommentService.Logic;
using CommentService.Models;
using Contracts;
using Contracts.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CommentService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly CommentLogic _commentLogic;
        
        public CommentController(CommentLogic commentLogic)
        {
            _commentLogic = commentLogic;
        }
        
        
        // get all comments for a specfic difficulty (general diff comments, beat-numbered comments)
        [HttpGet("all/difficulty/{difficultyDiscussionId}")]
        public async Task<ActionResult<GetAllDiffCommentsDto>> GetAllDifficultyComments(string difficultyDiscussionId)
        {
            LogicResponse<GetAllDiffCommentsDto> response = await _commentLogic.GetAllDifficultyCommentsAsync(difficultyDiscussionId);
            
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
                    throw new Exception($"Method GetAllDifficultyCommentsAsync at GetAllDifficultyComments(id) returned an unexpected response type. Message: {response.ErrorMessage}");
            }
            
            return Ok(response.Data);
        }
        
        
        // get all comments for a specific mapset (mapset comments, reviews)
        [HttpGet("all/mapset/{mapDiscussionId}")]
        public async Task<ActionResult<GetAllMapCommentsDto>> GetAllMapsetComments(string mapDiscussionId)
        {
            LogicResponse<GetAllMapCommentsDto> response = await _commentLogic.GetAllMapsetCommentsAsync(mapDiscussionId);
            
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
                    throw new Exception($"Method GetAllMapsetCommentsAsync at GetAllMapsetComments(id) returned an unexpected response type. Message: {response.ErrorMessage}");
            }
            
            return Ok(response.Data);
        }
        
        
        // get number of comments over the whole mapset by comment type / status
        [HttpGet("stats/{mapDiscussionId}/{difficultyDiscussionId}")]
        public async Task<ActionResult<CommentStatsDto>> GetCommentStats(string mapDiscussionId, string difficultyDiscussionId)
        {
            string? userId = User.Identity?.Name;
            
            LogicResponse<CommentStatsDto> response = await _commentLogic.GetCommentStatsAsync(mapDiscussionId, difficultyDiscussionId, userId);
            
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
                    throw new Exception($"Method GetCommentStatsAsync at GetCommentStats(id) returned an unexpected response type. Message: {response.ErrorMessage}");
            }
            
            return Ok(response.Data);
        }
        
        // get a comment by id
        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetCommentById(string id)
        {
            LogicResponse<Comment> response = await _commentLogic.GetCommentByIdAsync(id);
            
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
                    throw new Exception($"Method GetCommentByIdAsync at GetCommentById(id) returned an unexpected response type. Message: {response.ErrorMessage}");
            }
            
            return Ok(response.Data);
        }
        
        // add comment
        [Authorize(Roles = nameof(Role.User))]
        [HttpPost]
        public async Task<ActionResult<Comment>> AddComment(AddCommentDto comment)
        {
            string? userId = User.Identity?.Name;
            
            if (userId is null)
            {
                return Forbid();
            }
            
            LogicResponse<Comment> response = await _commentLogic.AddCommentAsync(comment, userId);

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
                    throw new Exception(
                        $"Method AddCommentAsync at AddComment(comment) returned an unexpected response type. Message: {response.ErrorMessage}");
            }
            
            return Ok(response.Data);
        }

        // edit comment
        [Authorize(Roles = nameof(Role.User))]
        [HttpPatch]
        public async Task<ActionResult<Comment>> EditComment(EditCommentDto comment)
        {
            string? userId = User.Identity?.Name;
            
            if (userId is null)
            {
                return Forbid();
            }
            
            LogicResponse<Comment> response = await _commentLogic.EditCommentAsync(comment, userId);

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
                    throw new Exception(
                        $"Method EditCommentAsync at EditComment(comment) returned an unexpected response type. Message: {response.ErrorMessage}");
            }
            
            return Ok(response.Data);
        }
        
        // delete comment
        [Authorize(Roles = nameof(Role.User))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(string id)
        {
            string? userId = User.Identity?.Name;
            bool isModerator = User.Claims.Any(c => c is { Type: ClaimTypes.Role, Value: nameof(Role.Moderator) });
            
            
            if (userId is null)
            {
                return Forbid();
            }
            
            LogicResponse<bool> response = await _commentLogic.DeleteCommentAsync(id, userId, isModerator);
            
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
                    throw new Exception(
                        $"Method DeleteCommentAsync at DeleteComment(id) returned an unexpected response type. Message: {response.ErrorMessage}");
            }
            
            return Ok();
        }
        
        // resolve comment
        [Authorize(Roles = nameof(Role.User))]
        [HttpPost("resolve/{id}")]
        public async Task<IActionResult> ResolveComment(string id)
        {
            string? userId = User.Identity?.Name;
            
            if (userId is null)
            {
                return Forbid();
            }
            
            LogicResponse<bool> response = await _commentLogic.ResolveCommentAsync(id, userId);
            
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
                    throw new Exception(
                        $"Method ResolveCommentAsync at ResolveComment(id) returned an unexpected response type. Message: {response.ErrorMessage}");
            }
            
            return Ok();
        }
        
        // reopen comment
        [Authorize(Roles = nameof(Role.User))]
        [HttpPost("reopen/{id}")]
        public async Task<IActionResult> ReopenComment(string id)
        {
            string? userId = User.Identity?.Name;
            
            if (userId is null)
            {
                return Forbid();
            }
            
            LogicResponse<bool> response = await _commentLogic.ReopenCommentAsync(id, userId);
            
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
                    throw new Exception(
                        $"Method ReopenCommentAsync at ReopenComment(id) returned an unexpected response type. Message: {response.ErrorMessage}");
            }
            
            return Ok();
        }
        
        
        
        // review controller
        
        // get a review by id
        
        // add review
        
        // edit review
        
        // delete review
    }
}
