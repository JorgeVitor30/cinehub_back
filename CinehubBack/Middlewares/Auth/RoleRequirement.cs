using System.Security.Claims;
using CinehubBack.Services.Auth;
using Microsoft.AspNetCore.Authorization;

namespace CinehubBack.Middlewares.Auth;

public class RoleRequirement : AuthorizationHandler<RoleRequirement>, IAuthorizationRequirement
{
    private readonly IAuthService _authService;

    public RoleRequirement(IAuthService authService)
    {
        _authService = authService;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RoleRequirement requirement
    )
    {
        var claims = context.User.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
        var roles = claims.Select(r => r.Value).ToList();

        System.Console.WriteLine(roles);

        if (_authService.ValidateRoles(roles))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        context.Fail();
        return Task.CompletedTask;
    }
}