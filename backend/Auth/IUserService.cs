using System.Threading.Tasks;

namespace AeroTrack.Api.Auth;

public interface IUserService
{
    Task<AuthUser?> Validate(string username, string password);
}