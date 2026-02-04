using System.Net;
using System.Text.Json;
using FluentValidation;
using Inventory.Application.Exceptions;

namespace Inventory.WebAPI.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message) = exception switch
        {
            NotFoundException => (HttpStatusCode.NotFound, exception.Message),
            BusinessException => (HttpStatusCode.BadRequest, exception.Message),
            ValidationException validationEx => (HttpStatusCode.BadRequest, string.Join("; ", validationEx.Errors.Select(e => e.ErrorMessage))),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred")
        };

        context.Response.StatusCode = (int)statusCode;

        var response = JsonSerializer.Serialize(new { message });
        await context.Response.WriteAsync(response);
    }
}
