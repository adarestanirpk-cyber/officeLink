#define Sync
//#define Async
using Application.DTOs;
using Application.Interfaces;
using Application.Mappers;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
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
    private readonly ILogger<WFCaselinkController> _logger;
    private readonly WFCaseDefaults _defaults;
    private readonly IConfiguration _configuration;
    private readonly IWFCaseLinkService _wFCaseLinkService;

    public WFCaselinkController(
        IWFCaseRepository repository,
        IFrontOfficeClient frontOfficeClient,
        ILogger<WFCaselinkController> logger,
        IConfiguration configuration,
        IOptions<WFCaseDefaults> defaults,
        IWFCaseLinkService wfCaseLinkService)
    {
        _repository = repository;
        _frontOfficeClient = frontOfficeClient;
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
        dto.Status = WFCaseLinkStatus.Pending;

        // تنظیم تاریخ ایجاد در صورت نبود مقدار
        dto.CreatedAt = DateTime.UtcNow;
    
        //save in database
        await _repository.AddAsync(dto.ToEntity(), ct);

        var cameFromBackOffice = Request.Headers.TryGetValue("X-Origin", out var origin) && origin == "BackOffice";

        //for avoiding loopback
        if (!cameFromBackOffice)
        {
            //send link
            try
            {
#if Sync
                //await _wFCaseLinkService.PublishLinkCreated(entity);
                //var boResult = await _frontOfficeClient.SendLinkAsync(dto, ct);
                //dto is received here so we update dto here then sending
                dto.Status = WFCaseLinkStatus.InProgress;
                dto.TargetCaseId = 6789;//random number
                dto.TargetMainEntityId = 8989;

                //entity.Status = boResult.Status;                 // معمولاً Completed
                //entity.TargetCaseId = boResult.TargetCaseId;     // اگر BO تعیین کند
                //entity.TargetMainEntityId = boResult.TargetMainEntityId;
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
        }

        return Ok(dto);
    }

    [HttpPost("callbo")]
    public async Task<IActionResult> CallBO(CancellationToken ct)
    {

        WFCaseLinkDto dto = new WFCaseLinkDto();
        dto.SourceCaseId = 5040;
        dto.SourceMainEntityId = 456;
        dto.SourceAppId = _defaults.SourceAppId;
        dto.SourceMainEntityName = "Asset Registeration Office1";
        dto.SourceWFClassName = "Asset Registering";

        dto.TargetAppId = Guid.NewGuid();
        dto.TargetMainEntityName = "Government Registeration office";
        dto.TargetWFClassName = "Government Asset Registering";

        dto.CreatedByUserId = 111;
        dto.LinkType = WFCaseLinkType.BOFO;

        try
        {
            // 🔹 مستقیم BO را صدا می‌زنیم (بدون ذخیره در دیتابیس FO)
            var result = await Create(dto, ct); // فراخوانی داخلی متد Create

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
