using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using CinehubBack.Expections;
using CinehubBack.Services.Auth;
using CinehubBack.Services.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CinehubBack.Middlewares.Auth;

public class BearerHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;

    public BearerHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder
        , IUserService userService, ITokenService tokenService)
        : base(options, logger, encoder)
    {
        _userService = userService;
        _tokenService = tokenService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var endpoint = Context.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
        {
            return AuthenticateResult.NoResult();
        }

        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return AuthenticateResult.Fail("Token expected is not present");
        }

        var authHeader = Request.Headers["Authorization"].ToString();
        if (!authHeader.StartsWith("Bearer "))
        {
            return AuthenticateResult.Fail("Invalid Authorization header");
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("your-secret-key-here-at-least-16-chars"); // Replace with your key
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "your-issuer",
                ValidAudience = "your-audience",
                IssuerSigningKey = new SymmetricSecurityKey(key)
            }, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var claims = jwtToken.Claims;
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }
        catch (Exception ex)
        {
            // Log the exception details
            Logger.LogError(ex, "Failed to extract claims from token");
            return AuthenticateResult.Fail($"It wasn't possible extract claims from token: {ex.Message}");
        }
    }

    private bool ShouldSkipAuth()
    {
        var requestPath = Context.Request.Path;
        var isHealthPath = requestPath.StartsWithSegments("/health");

        return isHealthPath;
    }

    private static string ExtractTokenFromHeader(IHeaderDictionary headers)
    {
        var authorizationHeader = headers["Authorization"];

        if (string.IsNullOrEmpty(authorizationHeader))
            return string.Empty;

        var authorization = authorizationHeader.ToString().Split(' ');
        return authorization[^1];
    }

    private AuthenticationTicket GetTicket(string token)
    {
        var claimsExtracted = _tokenService.ExtractFrom(token).ToList();
        var email = claimsExtracted.First(c => c.Type.Equals(ClaimTypes.Email)).Value;
        _ = _userService.GetByEmail(email);

        var identify = new ClaimsIdentity(claimsExtracted, nameof(BearerHandler));
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identify), Scheme.Name);

        return ticket;
    }
}