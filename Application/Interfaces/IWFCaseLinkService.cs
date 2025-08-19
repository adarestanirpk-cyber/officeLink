using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces;

public interface IWFCaseLinkService
{
    Task SendTestMessage();
    Task PublishLinkCreated(WFCaseLink entity);
    Task IncrementRetryAsync(WFCaseLinkDto dto, int retryAfterMinutes = 5);
    Task AddIncidentAsync(WFCaseLinkDto dto, string code, string message);
}
