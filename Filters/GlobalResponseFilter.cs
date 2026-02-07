using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PolicyManagement.Filters;

public class GlobalResponseFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var result = context.Result as ObjectResult;

        if (result != null)
        {
            var duration = context.HttpContext.Items.ContainsKey("RequestDurationMs")
                ? context.HttpContext.Items["RequestDurationMs"]
                : null;
            var isSuccess = result.StatusCode >= 200 && result.StatusCode < 300;
            var message = result.Value?.GetType().GetProperty("message")?.GetValue(result.Value, null) ?? "Request processed successfully";
            var data = result.Value?.GetType().GetProperty("data")?.GetValue(result.Value, null);
            var error = !isSuccess ? result.Value?.GetType().GetProperty("error")?.GetValue(result.Value, null) : null;
            var response = new
            {
                success = isSuccess,
                status_code = result.StatusCode,
                message,
                error,
                data,
                trace_id = context.HttpContext.TraceIdentifier,
                duration_ms = duration
            };
            context.Result = new JsonResult(response)
            {
                StatusCode = result.StatusCode
            };
        }

        await next();
    }
}