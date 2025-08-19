namespace BackOfficeAPI.Middleware;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string HeaderKey = "X-Correlation-Id";

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // اگر کلاینت CorrelationId فرستاد → همونو استفاده کن
        var correlationId = context.Request.Headers.TryGetValue(HeaderKey, out var cid)
            ? cid.ToString()
            : Guid.NewGuid().ToString();

        // در Response Header هم بذاریم (برای کلاینت)
        context.Response.Headers[HeaderKey] = correlationId;

        // داخل HttpContext ذخیره کنیم تا در کل لایه‌ها در دسترس باشه
        context.Items["CorrelationId"] = correlationId;

        // ادامه‌ی Pipeline
        await _next(context);
    }
}
