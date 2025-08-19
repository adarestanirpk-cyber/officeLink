namespace BackOfficeAPI.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // ادامه pipeline
        }
        catch (Exception ex)
        {
            // گرفتن CorrelationId از context (که قبلاً توسط CorrelationIdMiddleware اضافه شده)
            var correlationId = context.Items.ContainsKey("CorrelationId")
                ? context.Items["CorrelationId"]?.ToString()
                : Guid.NewGuid().ToString();

            _logger.LogError(ex, "Unhandled exception occurred. CorrelationId={CorrelationId}", correlationId);

            // پاسخ استاندارد به کلاینت
            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var result = new
            {
                error = "An unexpected error occurred.",
                correlationId
            };

            await context.Response.WriteAsJsonAsync(result);
        }
    }
}
