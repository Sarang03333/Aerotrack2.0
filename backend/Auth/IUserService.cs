using System.Threading.Tasks;

namespace AeroTrack.Api.Auth;

public interface IUserService
{
    Task<AuthUser?> ValidateAsync(string username, string password);
}