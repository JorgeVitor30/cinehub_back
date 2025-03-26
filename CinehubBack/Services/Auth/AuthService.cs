using System.Net;
using CinehubBack.Data.Auth;
using CinehubBack.Encrypt;
using CinehubBack.Expections;
using CinehubBack.Extensions;
using CinehubBack.Model;
using CinehubBack.Services.User;

namespace CinehubBack.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly IPasswordEncoder _passwordEncoder;
    private readonly ITokenService _tokenService;

    public AuthService(IUserService userService, IPasswordEncoder passwordEncoder, ITokenService tokenService)
    {
        _userService = userService;
        _passwordEncoder = passwordEncoder;
        _tokenService = tokenService;
    }

    public bool ValidateRoles(IList<string> roles)
    {
        return roles.Any(r =>
            r.Equals(Enum.GetName(typeof(Role), Role.Admin)) || r.Equals(Enum.GetName(typeof(Role), Role.User)));
    }

    public ResponseLoginDto Login(LoginDto loginDto)
    {
        var user = _userService.GetByEmail(loginDto.Email);
        if (user == null || !_passwordEncoder.Verify(loginDto.Password, user.Password))
            throw new BaseException(ErrorCode.InvalidCredentials, HttpStatusCode.BadRequest,
                "Email or Password incorrects");

        return new ResponseLoginDto { Token = _tokenService.Generate(user.GetClaims()) };
    }
}