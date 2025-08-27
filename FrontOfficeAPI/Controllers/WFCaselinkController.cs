#define Sync
//#define Async
using Application.DTOs;
using Application.Interfaces;
using Application.Mappers;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using FrontOfficeAPI.Extensions;
using Infrastructure.BackOffice.HttpClients;
using Infrastructure.FrontOffice.HttpClients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FrontOfficeAPI.Controllers;

[ApiController]
[Route("api/link")]
public class WFCaselinkController : Controller
{
    private readonly IWFCaseRepository _repository;
    private readonly IBackOfficeClient _backOfficeClient;
    private readonly IFrontOfficeClient _frontOfficeClient;
    private readonly ILogger<WFCaselinkController> _logger;
    private readonly IConfiguration _configuration;
    private readonly WFCaseDefaults _defaults;
    private readonly IWFCaseLinkService _wFCaseLinkService;
    private readonly IWorkflowService _workflowService;

    public WFCaselinkController(
        IWFCaseRepository repository,
        IBackOfficeClient backOfficeClient,
        IFrontOfficeClient frontOfficeClient,
        ILogger<WFCaselinkController> logger,
        IConfiguration configuration,
        IOptions<WFCaseDefaults> defaults,
        IWFCaseLinkService wFCaseLinkService,
        IWorkflowService workflowService
        )
    {
        _repository = repository;
        _backOfficeClient = backOfficeClient;
        _frontOfficeClient = frontOfficeClient;   
        _logger = logger;
        _configuration = configuration;
        _defaults = defaults.Value;
        _wFCaseLinkService = wFCaseLinkService;
        _workflowService = workflowService;
    }

    // POST: api/link in front
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] WFCaseLinkDto dto, CancellationToken ct)
    {
        if (dto == null)
            return BadRequest("DTO cannot be null.");

#if Sync
        try
        {
            var cameFromFront = Request.Headers.TryGetValue("X-Origin", out var origin) && origin == "FrontOffice";
            var cameFromFrontProxy = Request.Headers.TryGetValue("X-Origin", out origin) && origin == "FrontOfficeProxy";
            var cameFromBack = Request.Headers.TryGetValue("X-Origin", out origin) && origin == "BackOffice";
            var cameFromBackProxy = Request.Headers.TryGetValue("X-Origin", out origin) && origin == "BackOfficeProxy";

            // بررسی وجود رکورد قبلی در DB
            WFCaseLink? existing = null;
            // بررسی رکورد قبلی فقط اگر FO هستیم
            if ((cameFromFront || cameFromFrontProxy) && dto.currentTaskId != 0)
            {
                existing = await _repository.GetByTaskIdAsync(dto.currentTaskId, ct);
            }

            //secure data
            dto.Status = WFCaseLinkStatus.Created;
            // تنظیم تاریخ ایجاد در صورت نبود مقدار
            dto.CreatedAt = dto.CreatedAt == default ? DateTime.UtcNow : dto.CreatedAt;
            //save in database
            WFCaseLink entity = dto.ToEntity();
            //save in database
            await _repository.AddAsync(entity, ct);




            if (existing == null && !cameFromBack)
            {
                // ✅ هیچ رکوردی وجود ندارد → Create
                var result = await _workflowService.CallEngineAsync(
                    "domain",
                    dto.CreatedByUserId,
                    dto.TargetWFClassName,
                    dto.EntityJson);

                dto.TargetCaseId = result.TargetCaseId;
                dto.TargetMainEntityId = result.TargetMainEntityId;
                dto.Status = WFCaseLinkStatus.Created;
            }
            else if(existing != null && !cameFromBack)
            {
                // ✅ رکورد قبلی وجود دارد → Invoke
                dto.TargetCaseId = existing.TargetCaseId;
                dto.currentTaskId = existing.currentTaskId;

                var invokeDto = new InvokeProcessDto
                {
                    Domain = "Domain",
                    UserName = $"user-{dto.CreatedByUserId}",
                    IdCase = Convert.ToInt32(dto.TargetCaseId),
                    TaskId = Convert.ToInt32(dto.currentTaskId)
                };

                var result = await _workflowService.InvokeProcessAsync(invokeDto, TransitionType.Normal);
                dto.TargetMainEntityId = result.WorkflowResponse?.TargetMainEntityId ?? dto.TargetMainEntityId;
                dto.Status = WFCaseLinkStatus.Completed;
            }

            // ✅ ارسال لینک به طرف مقابل بدون ایجاد loopback
            if (dto.LinkType == WFCaseLinkType.FOBO)
            {
                // FO → BO
                if (cameFromBack || cameFromFrontProxy)
                {
                    dto = await _backOfficeClient.SendLinkAsync(dto, ct);
                }
            }
            else if (dto.LinkType == WFCaseLinkType.BOFO)
            {
                // BO → FO
                if (cameFromFront || cameFromBackProxy)
                {
                    dto = await _frontOfficeClient.SendLinkAsync(dto, ct);
                }
            }
            // به‌روزرسانی رکورد در دیتابیس
            await _repository.UpdateAsync(dto.ToEntity(), ct);
#elif Async

#endif
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CreateOrInvoke");
            //change the state to failed
            await _repository.UpdateWFStateToFailed(dto.ToEntity(), ct);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = "Error in CreateOrInvoke", details = ex.Message });
        }
        return Ok(dto);
    }

    [HttpPost("callbo")]
    public async Task<IActionResult> CallBO(CancellationToken ct)
    {
        //for mock data in json
        string json = @"{
          ""Title"": ""Test"",
          ""CreatedBy"": 5,
          ""Status"": 8,
          ""Type"": 9,
          ""Owener"": 11,
          ""Amirreza"": ""Taleb"",
          ""Amin"": ""Darestani"",
          ""AmirrezaCID"": 5
        }";

        //filling mock data
        WFCaseLinkDto dto = new()
        {
           SourceCaseId = 5040,
           SourceMainEntityId = 456,
           SourceAppId = _defaults.SourceAppId,
           SourceMainEntityName = "AssetRegisterationOffice1",
           SourceWFClassName = "AssetRegistering",
           CreatedAt = DateTime.UtcNow,
           TargetAppId = Guid.NewGuid(),
           TargetMainEntityName = "Taidddat",
           TargetWFClassName = "GovernmentAssetRegistering",
           CreatedByUserId = 111,
           LinkType = WFCaseLinkType.FOBO,
           EntityJson = json,
           currentTaskId = 159
        };

        try
        {
            if (!Request.Headers.ContainsKey("X-Origin"))
                Request.Headers.Add("X-Origin", "FrontOfficeProxy");

            var result = await Create(dto, ct); 
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CallBO while sending link to BackOffice");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = "Error sending link to BackOffice", details = ex.Message });
        }
    }

}
