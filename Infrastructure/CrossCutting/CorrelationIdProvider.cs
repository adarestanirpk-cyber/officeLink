using Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.CrossCutting;

public class CorrelationIdProvider : ICorrelationIdProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CorrelationIdProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string CorrelationId =>
       _httpContextAccessor.HttpContext?.Items["CorrelationId"]?.ToString()
       ?? Guid.NewGuid().ToString();
}
