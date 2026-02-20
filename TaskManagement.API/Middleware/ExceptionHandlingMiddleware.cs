using System.Net;
using System.Text.Json;

namespace TaskManagement.API.Middleware;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger
)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Pass the request to the next middleware
            await _next(context);
        }
        catch (Exception ex)
        {
            // Log the full exception internally — developers can see it
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);

            // But return a clean, controlled response to the client
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Map exception types to HTTP status codes
        // This is where you define your error contract
        var (statusCode, message, errors) = exception switch
        {
            FluentValidation.ValidationException vex => (
                HttpStatusCode.BadRequest,
                "Validation failed.",
                (object?)
                    vex
                        .Errors.GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
            ),
            InvalidOperationException => (
                HttpStatusCode.Conflict,
                exception.Message,
                (object?)null
            ),
            UnauthorizedAccessException => (
                HttpStatusCode.Unauthorized,
                "Unauthorized.",
                (object?)null
            ),
            KeyNotFoundException => (HttpStatusCode.NotFound, exception.Message, (object?)null),
            ArgumentException => (HttpStatusCode.BadRequest, exception.Message, (object?)null),
            _ => (
                HttpStatusCode.InternalServerError,
                "An unexpected error occurred.",
                (object?)null
            ),
        };
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            status = (int)statusCode,
            message,
            errors,
            timestamp = DateTime.UtcNow,
        };

        var json = JsonSerializer.Serialize(
            response,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
        );

        await context.Response.WriteAsync(json);
    }
}
