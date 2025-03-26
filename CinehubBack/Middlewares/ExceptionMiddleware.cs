using System.Net;
using System.Text.Json;
using CinehubBack.Expections;
using Microsoft.AspNetCore.Mvc;

namespace CinehubBack.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

        var result = JsonSerializer.Serialize(new
        {
            title = "Expected Exception",
            status = context.Response.StatusCode,
            detail = exception.Message,
            instance = context.Request.Path
        });

        return context.Response.WriteAsync(result);
    }
}