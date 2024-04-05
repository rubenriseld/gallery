namespace Gallery.API.DTOs;

public class LoginDTO
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}
