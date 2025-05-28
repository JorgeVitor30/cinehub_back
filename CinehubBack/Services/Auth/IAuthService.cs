using CinehubBack.Data.Auth;

namespace CinehubBack.Services.Auth;

public interface IAuthService
{
    public bool ValidateRoles(IList<string> roles);
    public ResponseLoginDto Login(LoginDto loginDtoA);
    public object Decode(string token);
}