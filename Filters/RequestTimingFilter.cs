using Microsoft.AspNetCore.Mvc.Filters;

namespace PolicyManagement.Filters;

public class RequestTimingFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var start = DateTime.UtcNow;
        Console.WriteLine($"Request started at: {start:O} for {context.HttpContext.Request.Method} {context.HttpContext.Request.Path}");
        var resultContext = await next();
        var end = DateTime.UtcNow;
        Console.WriteLine($"Request ended at: {end:O} for {context.HttpContext.Request.Method} {context.HttpContext.Request.Path}");
        var durationMs = (end - start).TotalMilliseconds;
        Console.WriteLine($"Request duration: {durationMs} ms for {context.HttpContext.Request.Method} {context.HttpContext.Request.Path}");
        context.HttpContext.Items["RequestDurationMs"] = durationMs;
    }
}
