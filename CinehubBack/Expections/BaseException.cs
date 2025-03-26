using System.Net;

namespace CinehubBack.Expections;

public class BaseException : Exception
{
    public string Code { get; init; }
    public HttpStatusCode Status { get; init; }

    public BaseException(string code, HttpStatusCode status, string message)
        : base(message)
    {
        Code = code;
        Status = status;
    }
}