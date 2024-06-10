using System.Net;
using System.Security.Claims;
using Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UserService.DTOs;
using UserService.Interfaces;
using UserService.Models;

namespace UserService.Logic;

public class LoginLogic
{
    private readonly UserDbContext _dbContext;
    private readonly IJwtProvider _jwtProvider;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IWebHostEnvironment _env;

    public LoginLogic(UserDbContext dbContext, IJwtProvider jwtProvider, IHttpClientFactory httpClientFactory, IWebHostEnvironment env)
    {
        _dbContext = dbContext;
        _jwtProvider = jwtProvider;
        _httpClientFactory = httpClientFactory;
        _env = env;
    }

    public async Task<LogicResponse<TokensDto>> HandleLoginAsync(LoginDto loginDto)
    {
        if (loginDto.AuthToken == null) return LogicResponse<TokensDto>.Fail("Invalid credentials", LogicResponseType.Unauthorized);
        string id;
        if (_env.IsDevelopment())
        {
            id = loginDto.Id;
        }
        else
        {
            try
            {
                id = await GetMainServerAuthorizationAndId(loginDto.AuthToken);
            }
            catch (Exception e)
            {
                if (e.Message == "Invalid authorization token.")
                {
                    return LogicResponse<TokensDto>.Fail("Invalid credentials", LogicResponseType.Unauthorized);
                }
                else if (e.GetType() == typeof(ArgumentException))
                {
                    return LogicResponse<TokensDto>.Fail("Empty authorization token", LogicResponseType.BadRequest);
                }

                throw;
            }
        }
        
        if (id != loginDto.Id)
        {
            return LogicResponse<TokensDto>.Fail("Invalid credentials combination", LogicResponseType.Unauthorized);
        }
        
        User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user is null)
        {
            return LogicResponse<TokensDto>.Fail("User not found", LogicResponseType.NotFound);
        }
        
        string token = _jwtProvider.GenerateJwtToken(user);
        string refreshToken = _jwtProvider.GenerateRefreshToken(user);
        
        TokensDto tokenDto = new TokensDto
        {
            JwtToken = token,
            RefreshToken = refreshToken
        };
        
        return LogicResponse<TokensDto>.Ok(tokenDto);
    }

    private async Task<string> GetMainServerAuthorizationAndId(string authToken)
    {
        if (string.IsNullOrEmpty(authToken))
            throw new ArgumentException("Value cannot be null or empty.", nameof(authToken));
        
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Cookie", authToken);
        
        var response =
            await client.GetAsync($"{Constants.blMainServerUrl}user/id");
        response.EnsureSuccessStatusCode();

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            throw new Exception("Invalid authorization token.");
        }

        var responseData = await response.Content.ReadAsStringAsync();

        return responseData;
    }
    
    public async Task<LogicResponse<TokensDto>> RefreshTokensAsync(TokensDto tokens)
    {
        string jwtToken = tokens.JwtToken;
        string refreshToken = tokens.RefreshToken;
        
        string userId;
        try
        {
            var principal = _jwtProvider.GetPrincipalFromExpiredToken(jwtToken);
            userId = principal.Identity?.Name!;
        }
        catch (SecurityTokenException e)
        {
            return LogicResponse<TokensDto>.Fail(e.Message, LogicResponseType.Unauthorized);
        }
        catch (ArgumentNullException)
        {
            return LogicResponse<TokensDto>.Fail("Token contains invalid data", LogicResponseType.BadRequest);
        }

        var savedRefreshToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.UserId == userId);

        if (savedRefreshToken == null || savedRefreshToken.ExpiryDate < DateTime.UtcNow)
        {
            return LogicResponse<TokensDto>.Fail("Invalid refresh token", LogicResponseType.Unauthorized);
        }
        
        User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
        {
            return LogicResponse<TokensDto>.Fail("User not found", LogicResponseType.NotFound);
        }

        var newAccessToken = _jwtProvider.GenerateJwtToken(user);
        var newRefreshToken = _jwtProvider.GenerateRefreshToken(user, savedRefreshToken);

        return LogicResponse<TokensDto>.Ok(new TokensDto
        {
            JwtToken = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }
    
    public async Task<LogicResponse<bool>> LogoutAsync(string userId, string refreshToken)
    {
        var savedRefreshToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.UserId == userId && rt.ExpiryDate > DateTime.UtcNow);
        
        if (savedRefreshToken is null)
        {
            return LogicResponse<bool>.Fail("Invalid refresh token", LogicResponseType.Unauthorized);
        }
        
        _dbContext.RefreshTokens.RemoveRange(_dbContext.RefreshTokens.Where(rt => rt.UserId == userId));
        await _dbContext.SaveChangesAsync();
        
        return LogicResponse<bool>.Ok(true);
    }
    
}