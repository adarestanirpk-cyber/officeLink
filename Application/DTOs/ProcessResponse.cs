using Domain.Enums;

namespace Application.DTOs;

public class ProcessResponse
{
    public bool IsSuccess { get; init; } = true;
    public string? Message { get; init; }
    public WorkflowResponseDto? WorkflowResponse { get; init; }
    public ProcessResponseType Type { get; init; }

    public static ProcessResponse Ok(string? message)
    {
        return new ProcessResponse
        { IsSuccess = true, Type = ProcessResponseType.Success, Message = message };
    }

    public static ProcessResponse Ok(WorkflowResponseDto? response)
    {
        return new ProcessResponse
        { IsSuccess = true, Type = ProcessResponseType.Success, WorkflowResponse = response };
    }

    public static ProcessResponse BadRequest(string? message)
    {
        return new ProcessResponse
        { IsSuccess = false, Type = ProcessResponseType.BadRequest, Message = message };
    }

    public static ProcessResponse Forbidden()
    {
        return new ProcessResponse { IsSuccess = false, Type = ProcessResponseType.Forbidden };
    }
}
