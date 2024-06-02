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
    public class ReviewController : ControllerBase
    {
        private readonly ReviewLogic _reviewLogic;

        public ReviewController(ReviewLogic reviewLogic)
        {
            _reviewLogic = reviewLogic;
        }

        // get all reviews by map discussion id
        [HttpGet("all/mapset/{mapDiscussionId}")]
        public async Task<ActionResult<List<Review>>> GetReviewsByMapDiscussionId(string mapDiscussionId)
        {
            LogicResponse<List<Review>> response = await _reviewLogic.GetReviewsByMapDiscussionIdAsync(mapDiscussionId);
            
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
                    throw new Exception($"Method GetReviewsByMapDiscussionIdAsync at GetReviewsByMapDiscussionId(id) returned an unexpected response type. Message: {response.ErrorMessage}");
            }
            
            return Ok(response.Data);
        }
        
        // get a review by id
        [HttpGet("{id}")]
        public async Task<ActionResult<Review>> GetReviewById(string id)
        {
            LogicResponse<Review> response = await _reviewLogic.GetReviewByIdAsync(id);
            
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
                    throw new Exception($"Method GetReviewByIdAsync at GetReviewById(id) returned an unexpected response type. Message: {response.ErrorMessage}");
            }
            
            return Ok(response.Data);
        }
        
        // add review
        [Authorize(Roles = nameof(Role.User))]
        [HttpPost]
        public async Task<ActionResult<Review>> AddReview(AddReviewDto review)
        {
            string? userId = User.Identity?.Name;
            
            if (userId is null)
            {
                return Forbid();
            }
            
            LogicResponse<Review> response = await _reviewLogic.AddReviewAsync(review, userId);
            
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
                    throw new Exception($"Method AddReviewAsync at AddReview() returned an unexpected response type. Message: {response.ErrorMessage}");
            }
            
            return Ok(response.Data);
        }
        
        // edit review
        [Authorize(Roles = nameof(Role.User))]
        [HttpPatch]
        public async Task<ActionResult<Review>> EditReview(EditReviewDto review)
        {
            string? userId = User.Identity?.Name;
            
            if (userId is null)
            {
                return Forbid();
            }
            
            LogicResponse<Review> response = await _reviewLogic.EditReviewAsync(review, userId);
            
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
                    throw new Exception($"Method EditReviewAsync at EditReview() returned an unexpected response type. Message: {response.ErrorMessage}");
            }
            
            return Ok(response.Data);
        }
        
        // delete review
        [Authorize(Roles = nameof(Role.User))]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Review>> DeleteReview(string id)
        {
            string? userId = User.Identity?.Name;
            bool isModerator = User.Claims.Any(c => c is { Type: ClaimTypes.Role, Value: nameof(Role.Moderator) });

            if (userId is null)
            {
                return Forbid();
            }

            LogicResponse<bool> response = await _reviewLogic.DeleteReviewAsync(id, userId, isModerator);

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
                        $"Method DeleteReviewAsync at DeleteReview(id) returned an unexpected response type. Message: {response.ErrorMessage}");
            }

            return Ok(response.Data);
        }

    }
}
