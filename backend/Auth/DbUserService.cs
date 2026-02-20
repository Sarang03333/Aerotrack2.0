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

    public async Task<AuthUser?> Validate(string username, string password)
    {
        // 1. Normalize input to lowercase to fix case-sensitivity issues
        var normalizedUser = username.ToLower();

        // 2. Query the Database
        var user = await _db.Users
            .SingleOrDefaultAsync(u => u.Username.ToLower() == normalizedUser);

        // 3. Check Password 
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