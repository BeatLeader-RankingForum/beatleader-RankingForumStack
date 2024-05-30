using System.Net;
using Contracts;
using Microsoft.EntityFrameworkCore;
using UserService.DTOs;
using UserService.Interfaces;
using UserService.Models;

namespace UserService.Logic;

public class LoginLogic
{
    private readonly AppDbContext _dbContext;
    private readonly IJwtProvider _jwtProvider;
    private readonly IHttpClientFactory _httpClientFactory;

    public LoginLogic(AppDbContext dbContext, IJwtProvider jwtProvider, IHttpClientFactory httpClientFactory)
    {
        _dbContext = dbContext;
        _jwtProvider = jwtProvider;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<LogicResponse<string>> HandleLoginAsync(LoginDto loginDto)
    {
        string id;
        try
        {
            id = await GetMainServerAuthorizationAndId(loginDto.AuthToken);
        }
        catch (Exception e)
        {
            if (e.Message == "Invalid authorization token.")
            {
                return LogicResponse<string>.Fail("Invalid credentials", LogicResponseType.Unauthorized);
            }

            throw;
        }
        
        if (id != loginDto.Id)
        {
            return LogicResponse<string>.Fail("Invalid credentials combination", LogicResponseType.Unauthorized);
        }
        
        User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user is null)
        {
            return LogicResponse<string>.Fail("User not found", LogicResponseType.NotFound);
        }
        
        string token = _jwtProvider.GenerateJwtToken(user);
        
        return LogicResponse<string>.Ok(token);
    }

    private async Task<string> GetMainServerAuthorizationAndId(string authToken)
    {
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
    
}