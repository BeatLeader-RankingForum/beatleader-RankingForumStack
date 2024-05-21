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

        public UserController(LoginLogic loginLogic, AppDbContext dbContext)
        {
            _loginLogic = loginLogic;
            _dbContext = dbContext;
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
    }
}
