using Expenses.Api.Infrastructure;

namespace Expenses.Api.Middleware
{
    public class TraceIdMiddleware
    {
        private readonly RequestDelegate _next;

        public TraceIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, TraceService traceService)
        {
            // Извлекаем TraceId из заголовка (стандартный или кастомный)
            var traceId = context.Request.Headers["traceparent"].FirstOrDefault() 
                         ?? context.Request.Headers["TraceId"].FirstOrDefault();
            
            traceService.SetTraceId(traceId);
            
            // Пробрасываем TraceId в ответ
            context.Response.Headers["TraceId"] = traceService.GetTraceId();
            
            await _next(context);
        }
    }
}