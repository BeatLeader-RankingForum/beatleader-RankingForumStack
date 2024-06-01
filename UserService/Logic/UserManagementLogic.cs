using System.Net;
using Contracts.Auth;
using UserService.DTOs;
using UserService.Models;
using Contracts;
using Newtonsoft.Json.Linq;

namespace UserService.Logic;

public class UserManagementLogic
{
    private readonly UserDbContext _dbContext;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<UserManagementLogic> _logger;
    

    public UserManagementLogic(UserDbContext dbContext, ILogger<UserManagementLogic> logger, IHttpClientFactory httpClientFactory)
    {
        _dbContext = dbContext;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
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
            var httpClient = _httpClientFactory.CreateClient();
            
            var response =
                await httpClient.GetAsync($"{Constants.blMainServerUrl}player/{id}?stats=false&keepOriginalId=false");
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
    
    public async Task<LogicResponse<User>> CreateUserFromData(CreateUserDto user)
    {
        // check for existing user with current id
        var existenceCheck = await _dbContext.Users.FindAsync(user.Id);
        if (existenceCheck != null)
        {
            return LogicResponse<User>.Fail("User already exists", LogicResponseType.Conflict);
        }
        
        string finalId;
        // Check main server for existence of player
        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            
            var response =
                await httpClient.GetAsync($"{Constants.blMainServerUrl}player/{user.Id}?stats=false&keepOriginalId=false");
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
        
        User newUser = new User()
        {
            Id = finalId,
            Roles = user.Roles
        };

        await _dbContext.Users.AddAsync(newUser);
        await _dbContext.SaveChangesAsync();

        return LogicResponse<User>.Ok(newUser);
    }

    public async Task<LogicResponse<User>> AddUserRoleAsync(UpdateUserRoleDto data)
    {
        var user = await _dbContext.Users.FindAsync(data.Id);
        if (user == null)
        {
            return LogicResponse<User>.Fail("User not found", LogicResponseType.NotFound);
        }

        if (user.Roles.Any(x => x == data.Role))
        {
            return LogicResponse<User>.Fail("User already has this role", LogicResponseType.Conflict);
        }

        user.Roles.Add(data.Role);
        await _dbContext.SaveChangesAsync();

        return LogicResponse<User>.Ok(user);
    }

    public async Task<LogicResponse<User>> RemoveUserRoleAsync(UpdateUserRoleDto data)
    {
        var user = await _dbContext.Users.FindAsync(data.Id);
        if (user == null)
        {
            return LogicResponse<User>.Fail("User not found", LogicResponseType.NotFound);
        }

        if (user.Roles.All(x => x != data.Role))
        {
            return LogicResponse<User>.Fail("User doesn't have this role", LogicResponseType.BadRequest);
        }

        user.Roles.Remove(data.Role);
        await _dbContext.SaveChangesAsync();

        return LogicResponse<User>.Ok(user);
    }

    public async Task<LogicResponse<User>> DeleteUserAsync(string id)
    {
        var user = await _dbContext.Users.FindAsync(id);
        if (user == null)
        {
            return LogicResponse<User>.Fail("User not found", LogicResponseType.NotFound);
        }

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();

        return LogicResponse<User>.Ok(user);
    }
    
}