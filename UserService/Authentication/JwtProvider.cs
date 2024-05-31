using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UserService.Interfaces;
using UserService.Models;

namespace UserService.Authentication;

public class JwtProvider : IJwtProvider
{
    private readonly JwtOptions _jwtOptions;
    private readonly AppDbContext _dbContext;
    private readonly JwtBearerOptions _jwtBearerOptions;

    public JwtProvider(IOptions<JwtOptions> options, AppDbContext dbContext, IOptions<JwtBearerOptions> jwtBearerOptions)
    {
        _dbContext = dbContext;
        _jwtBearerOptions = jwtBearerOptions.Value;
        _jwtOptions = options.Value;
    }

    public string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id)
        };
        
        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role.ToString())));
        
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)),
                SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(_jwtOptions.Issuer, _jwtOptions.Audience, claims, null, DateTime.UtcNow.AddMinutes(30), signingCredentials);

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
            savedRefreshToken.ExpiryDate = DateTime.UtcNow.AddDays(7);
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

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;

        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}