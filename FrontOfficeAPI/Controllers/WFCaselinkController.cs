using Application.DTOs;
using Application.Interfaces;
using Application.Mappers;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FrontOfficeAPI.Controllers;

[ApiController]
[Route("api/link")]
public class WFCaselinkController : Controller
{
    private readonly IWFCaseRepository _repository;
    private readonly IClient _frontOfficeClient;
    private readonly ILogger<WFCaselinkController> _logger;
    private readonly IConfiguration _configuration;
    private readonly WFCaseDefaults _defaults;
    private readonly IWFCaseLinkService _wFCaseLinkService;


    public WFCaselinkController(
        IWFCaseRepository repository,
        IClient frontOfficeClient,
        ILogger<WFCaselinkController> logger,
        IConfiguration configuration,
        IOptions<WFCaseDefaults> defaults,
        IWFCaseLinkService wFCaseLinkService)
    {
        _repository = repository;
        _frontOfficeClient = frontOfficeClient;
        _logger = logger;
        _configuration = configuration;
        _defaults = defaults.Value;
        _wFCaseLinkService = wFCaseLinkService;
    }

    // POST: api/link
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] WFCaseLinkDto dto, CancellationToken ct)
    {
        if (dto == null)
            return BadRequest("DTO cannot be null.");

        dto.Status = WFCaseLinkStatus.Created;

        //save in database
        WFCaseLink entity = dto.ToEntity();
        entity.Status = WFCaseLinkStatus.Created;
        await _repository.AddAsync(entity,ct);

        //update targetCaseId and mainEntityId
        // اگر MainEntityId داده نشده بود، یک عدد رندوم برای آن بساز
        if (entity.TargetMainEntityId == null)
        {
            var random = new Random();
            entity.TargetMainEntityId = random.Next(1000, 10000);
        }

        // اگر TargetCaseId داده نشده بود، یک عدد رندوم ۴ رقمی بساز
        if (entity.TargetCaseId == null)
        {
            var random = new Random();
            entity.TargetCaseId = random.Next(1000, 10000);
        }

        if (!Request.Headers.TryGetValue("X-Origin", out var origin) || origin != "FrontOffice")
        {
            //send link
            try
            {
                //fill data from api
                var responseDto = entity.ToDto();
                responseDto.Status = entity.Status;
                //create a random 4-digit number
                var random = new Random();
                responseDto.TargetCaseId = random.Next(1000, 10000);

                await _wFCaseLinkService.PublishLinkCreated(entity);

                //await _frontOfficeClient.SendLinkAsync(responseDto,ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending link to FrontOffice");
                //change the state to failed
                await _repository.UpdateWFStateToFailed(entity);
            }
        }
        return Ok(entity.ToDto());
    }

    [HttpPost("callfo")]
    public async Task<IActionResult> CallFO(CancellationToken ct)
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
        dto.LinkType = WFCaseLinkType.FOBO;

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
