using Microsoft.EntityFrameworkCore;
using AeroTrack.Api.Infrastructure;

namespace AeroTrack.Api.Auth;

public class DbUserService : IUserService
{
    private readonly AppDbContext _db;

    // Inject the Database Context
    public DbUserService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<AuthUser?> ValidateAsync(string username, string password)
    {
        // 1. Normalize input to lowercase to fix case-sensitivity issues
        var normalizedUser = username.ToLower();

        // 2. Query the Database
        // We look for a user where the stored username matches our input
        var user = await _db.Users
            .SingleOrDefaultAsync(u => u.Username.ToLower() == normalizedUser);

        // 3. Check Password 
        // (Note: In production, verify hash here: BCrypt.Verify(password, user.PasswordHash))
        if (user != null && user.Password == password)
        {
            return new AuthUser
            {
                UserId = user.UserId,
                Username = user.Username,
                Roles = user.Roles
            };
        }

        return null;
    }
}