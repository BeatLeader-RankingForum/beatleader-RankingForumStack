﻿namespace Contracts.Auth.Setup;

public class JwtOptions
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string SecretKey { get; set; }
    public int JwtExpiryInMinutes { get; set; }
    public int RefreshExpiryInDays { get; set; }
}