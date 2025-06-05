using CinehubBack.Data;
using CinehubBack.Data.Movie;
using CinehubBack.Data.Repositories;
using CinehubBack.Encrypt;
using CinehubBack.Middlewares.Auth;
using CinehubBack.Model;
using CinehubBack.Services.Auth;
using CinehubBack.Services.Favorite;
using CinehubBack.Services.Movie;
using CinehubBack.Services.Rate;
using CinehubBack.Services.User;
using Microsoft.AspNetCore.Authentication;

namespace CinehubBack.Extensions;

public static class DependencyInjectionExtensions
{
    public static void AddDependencies(this IServiceCollection services)
    {
        AddRepositories(services);
        AddServices(services);
        AddAdapters(services);
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IMovieService, MovieService>();
        services.AddScoped<IFavoriteService, FavoriteService>();
        services.AddScoped<IRateService, RateService>();
        
        services.AddScoped<IMovieFilter, TitleFilter>();
        services.AddScoped<IMovieFilter, GenreFilter>();
        services.AddScoped<IMovieFilter, VoteAverageFilter>();
        services.AddScoped<IMovieFilter, ExcludeRatedAndFavoriteFilter>();

        services.AddSingleton<ITokenService, JwtService>();
        services.AddSingleton<IPasswordEncoder, BCryptEncoder>();
        services.AddScoped<IAuthService, AuthService>();
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IRepository<User>, BaseRepository<User>>();
        services.AddScoped<IRepository<Movie>, BaseRepository<Movie>>();
        services.AddScoped<IRepository<Favorites>, BaseRepository<Favorites>>();
        services.AddScoped<IRepository<Rate>, BaseRepository<Rate>>();
    }

    private static void AddAdapters(IServiceCollection services) { }

    public static void AddCustomAuthorization(this IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        var authBusiness = provider.GetRequiredService<IAuthService>();
        services
            .AddAuthorizationBuilder()
            .AddDefaultPolicy(
                "Admin",
                policy => policy.AddRequirements(new RoleRequirement(authBusiness))
            );
    }

    public static void AddCustomAuthentication(this IServiceCollection services)
    {
        services.AddAuthenticationCore();
        services
            .AddAuthentication(o =>
            {
                o.DefaultScheme = Scheme.BearerAuthentication;
            })
            .AddScheme<AuthenticationSchemeOptions, BearerHandler>(
                Scheme.BearerAuthentication,
                _ => { }
            );
    }
}