using System.Net;
using Contracts.Auth;
using UserService.DTOs;
using UserService.Models;
using Contracts;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace UserService.Logic;

public class UserManagementLogic
{
    private readonly AppDbContext _dbContext;
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserManagementLogic> _logger;
    

    public UserManagementLogic(AppDbContext dbContext, HttpClient httpClient, ILogger<UserManagementLogic> logger)
    {
        _dbContext = dbContext;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<LogicResponse<User>> CreateUserFromIdAsync(string id)
    {
        // check for existing user with current id
        var existenceCheck = await _dbContext.Users.FindAsync(id);
        if (existenceCheck != null)
        {
            return LogicResponse<User>.Fail("User already exists", LogicResponseType.Conflict);
        }
        
        string finalId;
        // Check main server for existence of player
        try
        {
            var response =
                await _httpClient.GetAsync($"{Constants.blMainServerUrl}player/{id}?stats=false&keepOriginalId=false");
            response.EnsureSuccessStatusCode();

            var responseData = await response.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(responseData);
            finalId = json["id"]!.ToString();
        }
        catch (HttpRequestException e)
        {
            if (e.StatusCode == HttpStatusCode.NotFound)
            {
                return LogicResponse<User>.Fail("Player not found", LogicResponseType.NotFound);
            }

            _logger.LogError(e, "Error while checking main server for player existence");
            throw;
        }
        catch (NullReferenceException e)
        {
            _logger.LogError(e, "Failed to get ID from server response.");
            throw;
        }
        
        // check for existing user with final id
        var existenceCheck2 = await _dbContext.Users.FindAsync(finalId);
        if (existenceCheck2 != null)
        {
            return LogicResponse<User>.Fail("User already exists under migrated ID", LogicResponseType.Conflict);
        }
        
        User user = new User()
        {
            Id = finalId,
            Roles = new List<Role>() {Role.User}
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        return LogicResponse<User>.Ok(user);
    }
}