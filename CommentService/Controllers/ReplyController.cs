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
    public class ReplyController : ControllerBase
    {
        private readonly ReplyLogic _replyLogic;
        public ReplyController(ReplyLogic replyLogic)
        {
            _replyLogic = replyLogic;
        }

        // get all replies from comment
        [HttpGet("all/comment/{commentId}")]
        public async Task<ActionResult<List<Reply>>> GetAllRepliesToComment(string commentId)
        {
            LogicResponse<List<Reply>> response = await _replyLogic.GetAllRepliesToCommentAsync(commentId);
            
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
                    throw new Exception($"Method GetAllRepliesToCommentAsync at GetAllRepliesToComment(id) returned an unexpected response type. Message: {response.ErrorMessage}");
            }
            
            return Ok(response.Data);
        }
        
        // get reply by id
        [HttpGet("{id}")]
        public async Task<ActionResult<Reply>> GetReplyById(string id)
        {
            LogicResponse<Reply> response = await _replyLogic.GetReplyByIdAsync(id);
            
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
                    throw new Exception($"Method GetReplyByIdAsync at GetReplyById(id) returned an unexpected response type. Message: {response.ErrorMessage}");
            }
            
            return Ok(response.Data);
        }
        
        // add reply
        [Authorize(Roles = nameof(Role.User))]
        [HttpPost]
        public async Task<ActionResult<Reply>> AddReply(AddReplyDto reply)
        {
            string? userId = User.Identity?.Name;
            
            if (userId is null)
            {
                return Forbid();
            }
            
            LogicResponse<Reply> response = await _replyLogic.AddReplyAsync(reply, userId);
            
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
                    throw new Exception($"Method AddReplyAsync at AddReply(reply) returned an unexpected response type. Message: {response.ErrorMessage}");
            }
            
            return Ok(response.Data);
        }
        
        // edit reply
        [Authorize(Roles = nameof(Role.User))]
        [HttpPatch]
        public async Task<ActionResult<Reply>> EditReply(EditReplyDto reply)
        {
            string? userId = User.Identity?.Name;
            
            if (userId is null)
            {
                return Forbid();
            }
            
            LogicResponse<Reply> response = await _replyLogic.EditReplyAsync(reply, userId);
            
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
                    throw new Exception($"Method EditReplyAsync at EditReply(reply) returned an unexpected response type. Message: {response.ErrorMessage}");
            }
            
            return Ok(response.Data);
        }
        
        // delete reply
        [Authorize(Roles = nameof(Role.User))]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Reply>> DeleteReply(string id)
        {
            string? userId = User.Identity?.Name;
            bool isModerator = User.Claims.Any(c => c is { Type: ClaimTypes.Role, Value: nameof(Role.Moderator) });
            
            if (userId is null)
            {
                return Forbid();
            }
            
            LogicResponse<bool> response = await _replyLogic.DeleteReplyAsync(id, userId, isModerator);
            
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
                    throw new Exception($"Method DeleteReplyAsync at DeleteReply(id) returned an unexpected response type. Message: {response.ErrorMessage}");
            }
            
            return Ok(response.Data);
        }
        
        [HttpPost("LoadtestCleanup")]
        public async Task<IActionResult> LoadtestCleanup()
        {
            await _replyLogic.LoadtestCleanup();
            return Ok();
        }
    }
}
