namespace OrderPlatform.Api.Models.Auth;

public class AuthResponse
{
    public string Token { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Role { get; set; } = default!;
}
