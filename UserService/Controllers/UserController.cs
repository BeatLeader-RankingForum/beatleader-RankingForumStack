using Contracts;
using Contracts.Auth;
using Contracts.Auth.Setup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using UserService.Authentication;
using UserService.DTOs;
using UserService.Logic;
using UserService.Models;

namespace UserService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly LoginLogic _loginLogic;
        private readonly UserDbContext _dbContext;
        private readonly UserManagementLogic _userManagementLogic;
        private readonly JwtOptions _jwtOptions;
        private readonly IWebHostEnvironment _env;

        public UserController(LoginLogic loginLogic, UserDbContext dbContext, UserManagementLogic userManagementLogic, IOptions<JwtOptions> options, IWebHostEnvironment env)
        {
            _loginLogic = loginLogic;
            _dbContext = dbContext;
            _userManagementLogic = userManagementLogic;
            _jwtOptions = options.Value;
            _env = env;
        }
        
        [HttpPost("login")]
        public async Task<ActionResult<TokensDto>> Login(LoginDto loginDto)
        {
            string? mainServerToken = !(Request.Cookies[".AspNetCore.Cookies"]).IsNullOrEmpty() ? Request.Headers?.Cookie.ToString() : loginDto.AuthToken;

            if (mainServerToken is null && !_env.IsDevelopment())
            {
                return Unauthorized("No AuthToken from main server provided.");
            }

            loginDto.AuthToken = mainServerToken;
            
            LogicResponse<TokensDto> response = await _loginLogic.HandleLoginAsync(loginDto);

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
                    throw new Exception($"Method HandleLoginAsync at Login(loginDto) returned an unexpected response type. Message: {response.ErrorMessage}");
            }

            return Ok(response.Data);
        }
        
        [HttpPost("refresh")]
        public async Task<ActionResult<TokensDto>> Refresh(TokensDto tokens)
        {
            LogicResponse<TokensDto> response = await _loginLogic.RefreshTokensAsync(tokens);
            
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
                    throw new Exception($"Method RefreshTokensAsync at Refresh(tokens) returned an unexpected response type. Message: {response.ErrorMessage}");
            }
            
            return Ok(response.Data);
        }
        
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(TokensDto tokens)
        {
            string? userId = User.Identity?.Name;
            
            if (userId is null)
            {
                return BadRequest();
            }
            
            LogicResponse<bool> response = await _loginLogic.LogoutAsync(userId, tokens.RefreshToken);
            
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
                    throw new Exception($"Method LogoutAsync at Logout(tokens) returned an unexpected response type. Message: {response.ErrorMessage}");
            }
            
            return Ok();
        }
        
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<User>> GetMe()
        {
            if (User.Identity?.Name is null)
            {
                return NotFound();
            }
            
            User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == User.Identity.Name);

            if (user is null)
            {
                return NotFound();
            }
            
            return Ok(user);
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
                case LogicResponseType.Forbidden:
                    return Forbid(response.ErrorMessage!);
                default:
                    throw new Exception($"Method CreateUserFromIdAsync at CreateUser(id) returned an unexpected response type. Message: {response.ErrorMessage}");
            }

            return CreatedAtAction(nameof(GetUserById), new { id = response.Data!.Id }, response.Data);
        }
        
        [Authorize(Roles = nameof(Role.Admin))]
        [HttpPost("create/data")]
        public async Task<ActionResult<User>> CreateUser(CreateUserDto user)
        {
            LogicResponse<User> response = await _userManagementLogic.CreateUserFromData(user);

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
                    throw new Exception($"Method CreateUserFromIdAsync at CreateUser(id) returned an unexpected response type. Message: {response.ErrorMessage}");
            }

            return CreatedAtAction(nameof(GetUserById), new { id = response.Data!.Id }, response.Data);
        }

        [Authorize(Roles = nameof(Role.Admin))]
        [HttpPost("roles/add")]
        public async Task<ActionResult<User>> AddRole(UpdateUserRoleDto data)
        {
            LogicResponse<User> response = await _userManagementLogic.AddUserRoleAsync(data);

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
                    throw new Exception($"Method AddUserRoleAsync at AddRole(data) returned an unexpected response type. Message: {response.ErrorMessage}");
            }

            return Ok(response.Data);
        }
    
        [Authorize(Roles = nameof(Role.Admin))]
        [HttpPost("roles/remove")]
        public async Task<ActionResult<User>> RemoveRole(UpdateUserRoleDto data)
        {
            LogicResponse<User> response = await _userManagementLogic.RemoveUserRoleAsync(data);

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
                    throw new Exception($"Method RemoveUserRoleAsync at RemoveRole(data) returned an unexpected response type. Message: {response.ErrorMessage}");
            }

            return Ok(response.Data);
        }

        [Authorize(Roles = nameof(Role.Admin))]
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(string id)
        {
            LogicResponse<User> response = await _userManagementLogic.DeleteUserAsync(id);

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
                    throw new Exception($"Method DeleteUserAsync at DeleteUser(data) returned an unexpected response type. Message: {response.ErrorMessage}");
            }

            return Ok(response.Data);
        }

    }
}
