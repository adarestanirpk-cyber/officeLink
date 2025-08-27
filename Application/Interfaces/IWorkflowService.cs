using Application.DTOs;
using Domain.Enums;

namespace Application.Interfaces;

public interface IWorkflowService
{
    Task<WorkflowResponseDto> CallEngineAsync(string domainName, long userId, string targetWFClassName, string entityJson);
    Task<ProcessResponse> InvokeProcessAsync(InvokeProcessDto dto, TransitionType transitionType = TransitionType.Normal);
}
