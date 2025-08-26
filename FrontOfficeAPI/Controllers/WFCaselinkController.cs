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

    public WFCaselinkController(
        IWFCaseRepository repository,
        IBackOfficeClient backOfficeClient,
        IFrontOfficeClient frontOfficeClient,
        ILogger<WFCaselinkController> logger,
        IConfiguration configuration,
        IOptions<WFCaseDefaults> defaults,
        IWFCaseLinkService wFCaseLinkService)
    {
        _repository = repository;
        _backOfficeClient = backOfficeClient;
        _frontOfficeClient = frontOfficeClient;   
        _logger = logger;
        _configuration = configuration;
        _defaults = defaults.Value;
        _wFCaseLinkService = wFCaseLinkService;
    }

    // POST: api/link in front
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] WFCaseLinkDto dto, CancellationToken ct)
    {
        if (dto == null)
            return BadRequest("DTO cannot be null.");

        //secure data
        dto.Status = WFCaseLinkStatus.Created;
        // تنظیم تاریخ ایجاد در صورت نبود مقدار
        dto.CreatedAt = DateTime.UtcNow;
        //save in database
        WFCaseLink entity = dto.ToEntity();
        //save in database
        await _repository.AddAsync(entity, ct);

        //recognize source
        //var cameFromFront = dto.LinkType == WFCaseLinkType.FOBO;
        //var cameFromBack = dto.LinkType == WFCaseLinkType.BOFO;


        var cameFromFront = Request.Headers.TryGetValue("X-Origin", out var origin) && origin == "FrontOffice";
        var cameFromBack = Request.Headers.TryGetValue("X-Origin", out origin) && origin == "BackOffice";

        //for avoiding loopback
        //if (!(cameFromFront && IsFrontOffice) && !(cameFromBack && IsBackOffice))
        //{
#if Sync
        try
        {

            if (cameFromFront)
            {
                // منبع: FrontOffice → پس اینجا Back گیرنده است → callEngine
                var result = await callEnigne("domain", dto.CreatedByUserId, dto.TargetWFClassName, dto.EntityJson);
                dto.TargetCaseId = result.TargetCaseId;
                dto.TargetMainEntityId = result.TargetMainEntityId;
                dto.Status = WFCaseLinkStatus.Completed;
            }
            else if (cameFromBack)
            {
                // منبع: BackOffice → پس اینجا Front گیرنده است → callEngine
                var result = await callEnigne("domain", dto.CreatedByUserId, dto.TargetWFClassName, dto.EntityJson);
                dto.TargetCaseId = result.TargetCaseId;
                dto.TargetMainEntityId = result.TargetMainEntityId;
                dto.Status = WFCaseLinkStatus.Completed;
            }
            else
            {
                // درخواست داخلی (شروع‌کننده است)
                dto.Status = WFCaseLinkStatus.InProgress;

                if (dto.LinkType == WFCaseLinkType.FOBO)
                {
                    dto = await _backOfficeClient.SendLinkAsync(dto, ct);
                }
                else if (dto.LinkType == WFCaseLinkType.BOFO)
                {
                    dto = await _frontOfficeClient.SendLinkAsync(dto, ct);
                }
            }
            await _repository.UpdateAsync(dto.ToEntity(), ct);
#elif Async

#endif
        }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending link to FrontOffice");
                //change the state to failed
                await _repository.UpdateWFStateToFailed(dto.ToEntity(), ct);
            }
        //}
         return Ok(dto);
    }

    [HttpPost("callfo")]
    public async Task<IActionResult> CallFO(CancellationToken ct)
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
           EntityJson = json
        };

        try
        {
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

    private async Task<WorkflowResponseDto> callEnigne(string DomainName, long userId, string TargetWFClassName, string EntityJson)
    {
        //convert EntityJson to callAriaEngine input parameters

        //passing to entityFiel
        var entityFields = ToEntityFieldsClass.ToEntityFields(EntityJson);

        var dto = new CreateCaseDto
        {
            Domain = DomainName,
            UserName = $"user-{userId}",
            Process = TargetWFClassName,
            EntityFields = entityFields ?? new Dictionary<string, Dictionary<string, string>>()
        };

        ProcessResponse processResponse = await callAriaEnigne(dto);
        return new WorkflowResponseDto()
        {
            TargetCaseId = processResponse.WorkflowResponse!.TargetCaseId,
            TargetMainEntityId = processResponse.WorkflowResponse!.TargetMainEntityId
        };
    }

    private async Task<ProcessResponse> callAriaEnigne(CreateCaseDto dto)
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
