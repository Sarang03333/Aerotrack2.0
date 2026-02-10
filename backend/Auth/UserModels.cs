namespace AeroTrack.Api.Auth;

public class UserRecord
{
    public string UserId { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!; // DEMO ONLY 
    public string[] Roles { get; set; } = Array.Empty<string>();
}

public class AuthUser
{
    public string UserId { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string[] Roles { get; set; } = Array.Empty<string>();
}