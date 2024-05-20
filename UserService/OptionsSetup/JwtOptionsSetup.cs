using Microsoft.Extensions.Options;
using UserService.Authentication;

namespace UserService.OptionsSetup;

public class JwtOptionsSetup : IConfigureOptions<JwtOptions>
{
    public void Configure(JwtOptions options)
    {
        options.Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? throw new ArgumentNullException("JWT_ISSUER");
        options.Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? throw new ArgumentNullException("JWT_AUDIENCE");
        options.SecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ??
                            throw new ArgumentNullException("JWT_SECRET_KEY");
    }
}