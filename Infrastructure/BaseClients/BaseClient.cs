using System.Net.Http.Json;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.HttpClients;

public abstract class BaseClient
{
    protected readonly HttpClient _httpClient;
    protected readonly ILogger _logger;
    protected readonly ICorrelationIdProvider _correlationIdProvider;
    protected abstract string Origin { get; }

    protected BaseClient(HttpClient httpClient, ILogger logger, ICorrelationIdProvider correlationIdProvider)
    {
        _httpClient = httpClient;
        _logger = logger;
        _correlationIdProvider = correlationIdProvider;
    }

    protected async Task<WFCaseLinkDto> SendLinkInternalAsync(WFCaseLinkDto dto, CancellationToken ct)
    {
        var url = "/api/link";
        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(dto)
        };

        // 🔹 اضافه کردن هدرها
        request.Headers.Add("X-Correlation-Id", _correlationIdProvider.CorrelationId);
        request.Headers.Add("X-Origin", Origin);

        _logger.LogInformation(
            "Sending POST to {Url} with X-Origin={XOrigin}, CorrelationId={CorrelationId}",
            new Uri(_httpClient.BaseAddress!, url), Origin, _correlationIdProvider.CorrelationId);

        var response = await _httpClient.SendAsync(request, ct);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(ct);
            _logger.LogError("{Origin} returned error {StatusCode}: {Content}", Origin, response.StatusCode, content);
            throw new HttpRequestException($"{Origin} returned {response.StatusCode}: {content}");
        }

        var result = await response.Content.ReadFromJsonAsync<WFCaseLinkDto>(cancellationToken: ct);

        if (result == null)
            throw new InvalidOperationException($"{Origin} returned null DTO");

        return result;
    }
}
