using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using CinehubBack.Expections;
using Microsoft.IdentityModel.Tokens;

namespace CinehubBack.Services.Auth;

public class JwtService : ITokenService
{
    private readonly string _keySecret;

    public JwtService(IConfiguration configuration)
    {
        _keySecret = configuration["Secret:TokenKey"] ?? throw new Exception("'appsettings' without Secret:TokenKey");
    }

    public string Generate(IEnumerable<Claim> claims)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_keySecret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(8),
            SigningCredentials =
                new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public IEnumerable<Claim> ExtractFrom(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_keySecret);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = true
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            if (principal == null || validatedToken == null)
            {
                throw new BaseException(ErrorCode.ExtractFromToken, HttpStatusCode.Unauthorized,
                    "It wasn't possible extract claims from token");
            }

            return principal.Claims.ToList();
        }
        catch
        {
            throw new BaseException(ErrorCode.ExtractFromToken, HttpStatusCode.Unauthorized,
                "It wasn't possible extract claims from token");
        }
    }
    
    public string? GetUserIdFromToken(string token)
    {
        var claims = ExtractFrom(token);
        var userId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        return userId;
    }
}