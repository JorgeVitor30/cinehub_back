using System.Net;

namespace CinehubBack.Expections;

public class BaseException : Exception
{
    public string Code { get; }
    public HttpStatusCode Status { get; }

    public BaseException(string code, HttpStatusCode statusCode, string message)
        : base(message)
    {
        Code = code;
        Status= statusCode;
    }
}