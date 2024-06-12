using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Contracts.Auth.OptionsSetup;

public class AuthorizeOptionsSetup : IConfigureNamedOptions<AuthorizationOptions>
{
    public void Configure(AuthorizationOptions options)
    {
        foreach (Role role in Enum.GetValues(typeof(Role)))
        {
            options.AddPolicy(role.ToString(), policy =>
            {
                policy.RequireRole(role.ToString());
            });
        }
    }

    public void Configure(string? name, AuthorizationOptions options) => Configure(options);
}