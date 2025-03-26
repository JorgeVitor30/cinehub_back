namespace CinehubBack.Expections;

public static class ErrorCode
{
    public const string InvalidCredentials = "InvalidCredentials";
    
    public const string ExtractFromToken = "ExtractFromToken";
    
    public static string BadRequest() => "BadRequest";

    public static string NotFound<T>() => $"NotFound_{typeof(T).Name}";

    public static string Conflict<T>() => $"Conflict_{typeof(T).Name}";

    public static string Unauthenticated() => "Unauthenticated";
}