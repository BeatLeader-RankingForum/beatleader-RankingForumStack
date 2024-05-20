using Microsoft.Extensions.Options;
using UserService.Authentication;

namespace UserService.OptionsSetup;

public class JwtOptionsSetup : IConfigureOptions<JwtOptions>
{
    public void Configure(JwtOptions options)
    {
        options.Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "UserService";
        options.Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "RankingForumStack";
        options.SecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ??
                            throw new ArgumentNullException("JWT_SECRET_KEY");

        if (options.SecretKey.Length < 32)
        {
            throw new ArgumentException("The JWT secret key must be at least 32 characters long.");
        }
    }
}