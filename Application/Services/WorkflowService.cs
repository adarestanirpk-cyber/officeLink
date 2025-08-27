using Application.DTOs;
using Application.Interfaces;
using Application.Mappers;
using Domain.Enums;

namespace Application.Services;

public class WorkflowService: IWorkflowService
{
    public async Task<WorkflowResponseDto> CallEngineAsync(string domainName, long userId, string targetWFClassName, string entityJson)
    {
        // convert EntityJson to callAriaEngine input parameters
        var entityFields = ToEntityFieldsClass.ToEntityFields(entityJson);

        var dto = new CreateCaseDto
        {
            Domain = domainName,
            UserName = $"user-{userId}",
            Process = targetWFClassName,
            EntityFields = entityFields ?? new Dictionary<string, Dictionary<string, string>>()
        };

        ProcessResponse processResponse = await CallAriaEngine(dto);

        return new WorkflowResponseDto
        {
            TargetCaseId = processResponse.WorkflowResponse!.TargetCaseId,
            TargetMainEntityId = processResponse.WorkflowResponse!.TargetMainEntityId
        };
    }

    public async Task<ProcessResponse> InvokeProcessAsync(InvokeProcessDto dto, TransitionType transitionType = TransitionType.Normal)
    {
        // اینجا باید در واقع call بزنی به همون Engine اصلی
        // (یا از طریق یک client اگه microservice باشه، یا مستقیم new EngineUtility.InvokeProcess)

        // نسخه ساده Mock:
        return await Task.FromResult(new ProcessResponse
        {
            WorkflowResponse = new WorkflowResponseDto
            {
                TargetCaseId = dto.IdCase,
                TargetMainEntityId = new Random().Next(1000, 10000)
            }
        });
    }

    private async Task<ProcessResponse> CallAriaEngine(CreateCaseDto dto)
    {
        Random rnd = new Random();

        ProcessResponse processResponse = new()
        {
            WorkflowResponse = new WorkflowResponseDto()
            {
                TargetCaseId = rnd.Next(1000, 10000),
                TargetMainEntityId = rnd.Next(1000, 10000)
            }
        };

        return processResponse;
    }

}
