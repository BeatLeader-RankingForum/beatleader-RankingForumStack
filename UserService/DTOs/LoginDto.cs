namespace UserService.DTOs;

public class LoginDto
{
    public required string Id { get; set; }
    public required string AuthToken { get; set; }
}