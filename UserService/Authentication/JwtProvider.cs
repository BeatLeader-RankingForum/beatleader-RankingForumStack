using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Contracts.Auth.Setup;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using UserService.Interfaces;
using UserService.Models;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace UserService.Authentication;

public class JwtProvider : IJwtProvider
{
    private readonly JwtOptions _jwtOptions;
    private readonly UserDbContext _dbContext;
    private readonly JwtBearerOptions _jwtBearerOptions;

    public JwtProvider(IOptions<JwtOptions> options, UserDbContext dbContext, IOptions<JwtBearerOptions> jwtBearerOptions)
    {
        _dbContext = dbContext;
        _jwtBearerOptions = jwtBearerOptions.Value;
        _jwtOptions = options.Value;
    }

    public string GenerateJwtToken(User user)
    {
        if (user.Id.IsNullOrEmpty())
        {
            throw new ArgumentException("user.Id must not be null or empty", user.Id);
        }
        
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id)
        };
        
        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role.ToString())));
        
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)),
                SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(_jwtOptions.Issuer, _jwtOptions.Audience, claims, null, DateTime.UtcNow.AddMinutes(_jwtOptions.JwtExpiryInMinutes), signingCredentials);

        string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenValue;
    }

    public string GenerateRefreshToken(User user, RefreshToken? savedRefreshToken = null)
    {
        string token;
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            token = Convert.ToBase64String(randomNumber);
        }
        
        if (savedRefreshToken != null)
        {
            savedRefreshToken.Token = token;
            savedRefreshToken.ExpiryDate = DateTime.UtcNow.AddDays(_jwtOptions.RefreshExpiryInDays);
            _dbContext.RefreshTokens.Update(savedRefreshToken);
        }
        else
        {
            _dbContext.RefreshTokens.Add(new RefreshToken
            {
                UserId = user.Id,
                Token = token,
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            });
        }
        
        _dbContext.SaveChanges();
        
        return token;
    }
    
    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = _jwtBearerOptions.TokenValidationParameters.Clone();
        tokenValidationParameters.ValidateLifetime = false; // Allow expired tokens

        var tokenHandler = new JsonWebTokenHandler();
        tokenHandler.InboundClaimTypeMap.Clear();
        
        var result = tokenHandler.ValidateTokenAsync(token, tokenValidationParameters).Result;
    
        if (!result.IsValid)
        {
            throw new SecurityTokenException("Invalid token");
        }

        var jwtSecurityToken = result.SecurityToken as JsonWebToken;
    
        if (jwtSecurityToken == null || !jwtSecurityToken.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }
        
        return new ClaimsPrincipal(result.ClaimsIdentity);
    }
}