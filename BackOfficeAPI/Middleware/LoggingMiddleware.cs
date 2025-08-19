namespace BackOfficeAPI.Middleware;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // گرفتن CorrelationId از Middleware قبلی
        var correlationId = context.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString();

        // لاگ درخواست ورودی
        _logger.LogInformation(
            "Incoming Request {Method} {Path} CorrelationId={CorrelationId}",
            context.Request.Method,
            context.Request.Path,
            correlationId
        );

        // ادامه Pipeline
        await _next(context);

        // لاگ پاسخ خروجی
        _logger.LogInformation(
            "Outgoing Response {StatusCode} {Path} CorrelationId={CorrelationId}",
            context.Response.StatusCode,
            context.Request.Path,
            correlationId
        );
    }
}
