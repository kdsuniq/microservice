using AuthService.Core.Infrastructure;

namespace AuthService.Api.Middleware;

public class TraceIdMiddleware
{
    private readonly RequestDelegate _next;

    public TraceIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, TraceService traceService)
    {
        var traceId = context.Request.Headers["TraceId"].FirstOrDefault();
        traceService.SetTraceId(traceId);
        
        context.Response.Headers["TraceId"] = traceService.GetTraceId();
        
        await _next(context);
    }
}