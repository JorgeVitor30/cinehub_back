using System.Security.Claims;

namespace CinehubBack.Services.Auth;

public interface ITokenService
{
    string Generate(IEnumerable<Claim> claims);
    IEnumerable<Claim> ExtractFrom(string token);
    string? GetUserIdFromToken(string token);
}