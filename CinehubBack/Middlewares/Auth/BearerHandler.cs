using System.Security.Claims;
using System.Text.Encodings.Web;
using CinehubBack.Services.Auth;
using CinehubBack.Services.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

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

        try
        {
            var token = ExtractTokenFromHeader(Request.Headers);
            var ticket = GetTicket(token);
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