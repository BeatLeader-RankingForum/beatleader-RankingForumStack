using Contracts.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Authentication;
using UserService.DTOs;
using UserService.Logic;
using UserService.Models;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly LoginLogic _loginLogic;
        private readonly AppDbContext _dbContext;
        private readonly UserManagementLogic _userManagementLogic;

        public UserController(LoginLogic loginLogic, AppDbContext dbContext, UserManagementLogic userManagementLogic)
        {
            _loginLogic = loginLogic;
            _dbContext = dbContext;
            _userManagementLogic = userManagementLogic;
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            LogicResponse<string> response = await _loginLogic.HandleLoginAsync(loginDto);

            if (response.Type == LogicResponseType.NotFound)
            {
                return NotFound(response.ErrorMessage);
            }

            if (response.Type == LogicResponseType.Unauthorized)
            {
                return Unauthorized(response.ErrorMessage);
            }

            if (response.Type == LogicResponseType.BadRequest)
            {
                return BadRequest(response.ErrorMessage);
            }

            return Ok(response.Data);
        }
        
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(string id)
        {
            User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user is null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [Authorize(Roles = nameof(Role.User))]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            return await _dbContext.Users.ToListAsync();
        }
        
        [HttpPost("create/{id}")]
        public async Task<ActionResult<User>> CreateUser(string id)
        {
            LogicResponse<User> response = await _userManagementLogic.CreateUserFromIdAsync(id);

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
                default:
                    throw new Exception($"Method CreateUserFromIdAsync at CreateUser(id) returned an unexpected response type. Message: {response.ErrorMessage}");
            }

            return CreatedAtAction(nameof(GetUserById), new { id = response.Data!.Id }, response.Data);
        }
        
        [Authorize(Roles = nameof(Role.Admin))]
        [HttpPost("create/full")]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            throw new NotImplementedException();
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }
        
    }
}
