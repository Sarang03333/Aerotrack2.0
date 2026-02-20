namespace AeroTrack.Api.Auth;

public class AuthUser
{
    public string UserId { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string[] Roles { get; set; } = Array.Empty<string>();
}