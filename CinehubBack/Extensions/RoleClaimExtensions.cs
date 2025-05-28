using System.Security.Claims;
using CinehubBack.Model;

namespace CinehubBack.Extensions;


public static class RoleClaimExtention
{
    public static IEnumerable<Claim> GetClaims(this User user)
    {
        var result = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, Enum.GetName(typeof(Role), user.Role) ?? "NoRole")
        };
        return result;
    }
}