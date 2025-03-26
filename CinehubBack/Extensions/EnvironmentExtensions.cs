namespace CinehubBack.Extensions;

public static class EnvironmentExtensions
{
    public static bool IsLocal(this IWebHostEnvironment env)
    {
        return env.IsEnvironment("Local");
    }
}