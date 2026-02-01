using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace AeroTrack.Api.Auth;

public class InMemoryUserService : IUserService
{
    // DEMO USERS (Admin, Maintenance, InventoryManager, ComplianceOfficer)
    private static readonly ConcurrentDictionary<string, UserRecord> _users = new(new[]
    {
        new KeyValuePair<string, UserRecord>("admin", new UserRecord { UserId="u1", Username="admin", Password="P@ssw0rd!", Roles=new[]{ "Admin" } }),
        new KeyValuePair<string, UserRecord>("maint", new UserRecord { UserId="u2", Username="maint", Password="P@ssw0rd!", Roles=new[]{ "Maintenance" } }),
        new KeyValuePair<string, UserRecord>("inv",   new UserRecord { UserId="u3", Username="inv",   Password="P@ssw0rd!", Roles=new[]{ "InventoryManager" } }),
        new KeyValuePair<string, UserRecord>("comp",  new UserRecord { UserId="u4", Username="comp",  Password="P@ssw0rd!", Roles=new[]{ "ComplianceOfficer" } })
    });

    public Task<AuthUser?> ValidateAsync(string username, string password)
    {
        if (_users.TryGetValue(username, out var user) && user.Password == password)
        {
            return Task.FromResult<AuthUser?>(new AuthUser
            {
                UserId = user.UserId,
                Username = user.Username,
                Roles = user.Roles
            });
        }
        return Task.FromResult<AuthUser?>(null);
    }
}