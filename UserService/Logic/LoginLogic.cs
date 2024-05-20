using Microsoft.EntityFrameworkCore;
using UserService.DTOs;
using UserService.Interfaces;
using UserService.Models;

namespace UserService.Logic;

public class LoginLogic
{
    private readonly AppDbContext _dbContext;
    private readonly IJwtProvider _jwtProvider;

    public LoginLogic(AppDbContext dbContext, IJwtProvider jwtProvider)
    {
        _dbContext = dbContext;
        _jwtProvider = jwtProvider;
    }

    public async Task<LogicResponse<string>> HandleLoginAsync(LoginDto loginDto)
    {
        User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == loginDto.Id);

        if (user is null)
        {
            return LogicResponse<string>.Fail("User not found", LogicResponseType.NotFound);
        }
        
        // TODO: check auth with main server
        
        
        string token = _jwtProvider.GenerateJwtToken(user);
        
        return LogicResponse<string>.Ok(token);
    }
}