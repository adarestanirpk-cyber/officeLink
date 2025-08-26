#define Sync
//#define Async
using System;
using System.Text.Json;
using Application.DTOs;
using Application.Interfaces;
using Application.Mappers;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using Infrastructure.FrontOffice.HttpClients;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BackOfficeAPI.Controllers;

[ApiController]
[Route("api/link")]
public class WFCaselinkController : Controller
{
    private readonly IWFCaseRepository _repository;
    private readonly IFrontOfficeClient _frontOfficeClient;
    private readonly IBackOfficeClient _backOfficeClient;
    private readonly ILogger<WFCaselinkController> _logger;
    private readonly WFCaseDefaults _defaults;
    private readonly IConfiguration _configuration;
    private readonly IWFCaseLinkService _wFCaseLinkService;

    public WFCaselinkController(
        IWFCaseRepository repository,
        IFrontOfficeClient frontOfficeClient,
        IBackOfficeClient backOfficeClient,
        ILogger<WFCaselinkController> logger,
        IConfiguration configuration,
        IOptions<WFCaseDefaults> defaults,
        IWFCaseLinkService wfCaseLinkService)
    {
        _repository = repository;
        _frontOfficeClient = frontOfficeClient;
        _backOfficeClient = backOfficeClient;
        _logger = logger;
        _configuration = configuration;
        _defaults = defaults.Value;
        _wFCaseLinkService = wfCaseLinkService;
    }

    // POST: api/link in back
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

        var cameFromFront = Request.Headers.TryGetValue("X-Origin", out var origin) && origin == "FrontOffice";
        var cameFromBack = Request.Headers.TryGetValue("X-Origin", out origin) && origin == "BackOffice";
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
            SourceCaseId = 8085,
            SourceMainEntityId = 569,
            SourceAppId = _defaults.SourceAppId,
            SourceMainEntityName = "Taidddat",
            SourceWFClassName = "AssetRegistering",
            CreatedAt = DateTime.UtcNow,
            TargetAppId = Guid.NewGuid(),
            TargetMainEntityName = "GovernmentAssetRegistering",
            TargetWFClassName = "AssetRegisterationOffice1",
            CreatedByUserId = 121,
            LinkType = WFCaseLinkType.BOFO,
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

        //WFCaseLinkDto dto = new WFCaseLinkDto();
        //dto.SourceCaseId = 5040;
        //dto.SourceMainEntityId = 456;
        //dto.SourceAppId = _defaults.SourceAppId;
        //dto.SourceMainEntityName = "Asset Registeration Office1";
        //dto.SourceWFClassName = "Asset Registering";

        //dto.TargetAppId = Guid.NewGuid();
        //dto.TargetMainEntityName = "Government Registeration office";
        //dto.TargetWFClassName = "Government Asset Registering";

        //dto.CreatedByUserId = 111;
        //dto.LinkType = WFCaseLinkType.FOBO;

        ////for mock data in json
        //string json = @"{
        //  ""domain"": ""domain"",
        //  ""userName"": ""adarestani"",
        //  ""process"": ""process1"",
        //  ""entityFields"": {
        //    ""additionalProp1"": {
        //      ""additionalProp1"": ""item1"",
        //      ""additionalProp2"": ""item2"",
        //      ""additionalProp3"": ""item3""
        //    },
        //    ""additionalProp2"": {
        //      ""additionalProp1"": ""item4"",
        //      ""additionalProp2"": ""item5"",
        //      ""additionalProp3"": ""item6""
        //    },
        //    ""additionalProp3"": {
        //      ""additionalProp1"": ""item7"",
        //      ""additionalProp2"": ""item8"",
        //      ""additionalProp3"": ""item9""
        //    }
        //  }
        //}";
        //dto.EntityJson = json;

        //try
        //{
        //    // 🔹 مستقیم BO را صدا می‌زنیم (بدون ذخیره در دیتابیس FO)
        //    var result = await Create(dto, ct); // فراخوانی داخلی متد Create

        //    return Ok(result);
        //}
        //catch (Exception ex)
        //{
        //    _logger.LogError(ex, "Error in CallBO while sending link to BackOffice");
        //    return StatusCode(StatusCodes.Status500InternalServerError,
        //        new { error = "Error sending link to BackOffice", details = ex.Message });
        //}
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

        ProcessResponse processResponse = new ()
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
