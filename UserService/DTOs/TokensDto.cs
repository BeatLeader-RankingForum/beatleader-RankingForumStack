namespace UserService.DTOs;

public class TokensDto
{
    public required string JwtToken { get; set; }
    public required string RefreshToken { get; set; }
}