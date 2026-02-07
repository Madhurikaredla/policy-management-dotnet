using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PolicyManagement.Filters;

/// <summary>
/// Global exception filter for handling unhandled exceptions
/// </summary>
public class ExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ExceptionFilter> logger;

    public ExceptionFilter(ILogger<ExceptionFilter> logger)
    {
        this.logger = logger;
    }

    public void OnException(ExceptionContext context)
    {

       Console.WriteLine($"[ExceptionFilter] {context.Exception.GetType().Name}: {context.Exception.Message}");

        logger.LogError(context.Exception, "Unhandled exception occurred: {Message}", context.Exception.Message);

        var statusCode = context.Exception switch
        {
            InvalidOperationException => 400,
            UnauthorizedAccessException => 401,
            ArgumentException => 400,
            _ => 500
        };

        var startTime = context.HttpContext.Items["RequestStartTime"] as DateTime? ?? DateTime.UtcNow;
        var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;

        var genericMsg = context.Exception switch
        {
            InvalidOperationException => "Registration failed",
            UnauthorizedAccessException => "Unauthorized",
            ArgumentException => "Invalid argument",
            _ => "An error occurred"
        };
        var response = new
        {
            success = false,
            status_code = statusCode,
            message = $"{genericMsg}: {context.Exception.Message}",
            error = context.Exception.Message,
            data = (object?)null,
            trace_id = context.HttpContext.TraceIdentifier,
            duration_ms = duration
        };

        context.Result = new JsonResult(response)
        {
            StatusCode = statusCode
        };

        context.ExceptionHandled = true;
        logger.LogInformation("Exception handled with status code {StatusCode}", statusCode);
    }
}
