namespace UserService.DTOs;

public class LoginDto
{
    public required string Id { get; set; }
    public string? AuthToken { get; set; }
}