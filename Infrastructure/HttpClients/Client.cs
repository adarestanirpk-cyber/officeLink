using System.Net.Http.Json;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Infrastructure.HttpClients;

public class Client: IClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<Client> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public Client(HttpClient httpClient, ILogger<Client> logger, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<WFCaseLinkDto> SendLinkAsync(WFCaseLinkDto dto, CancellationToken ct)
    {
        var url = "/api/link";
        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(dto)
        };

        // 🔹 گرفتن CorrelationId از Middleware
        var correlationId = _httpContextAccessor.HttpContext?.Items["CorrelationId"]?.ToString()
                            ?? Guid.NewGuid().ToString(); // fallback در صورت نبود

        // 🔹 اضافه کردن هدرها
        request.Headers.Add("X-Correlation-Id", correlationId);
        request.Headers.Add("X-Origin", "FrontOffice");

        _logger.LogInformation("Sending POST to {Url} with X-Origin={XOrigin}, CorrelationId={CorrelationId}",
            new Uri(_httpClient.BaseAddress, url), "FrontOffice", correlationId);
        try
        {
            var response = await _httpClient.SendAsync(request, ct);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(ct);
                _logger.LogError("BackOffice returned error {StatusCode}: {Content}", response.StatusCode, content);
                throw new HttpRequestException($"BackOffice returned {response.StatusCode}: {content}");
            }

            var result = await response.Content.ReadFromJsonAsync<WFCaseLinkDto>(cancellationToken: ct);

            if (result == null)
                throw new InvalidOperationException("BackOffice returned null DTO");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling BackOffice API");
            throw;
        }
    }
}
