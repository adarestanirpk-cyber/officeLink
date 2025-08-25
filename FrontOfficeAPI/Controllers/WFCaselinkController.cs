//#define Sync
#define Async
using Application.DTOs;
using Application.Interfaces;
using Application.Mappers;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
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
    private readonly ILogger<WFCaselinkController> _logger;
    private readonly IConfiguration _configuration;
    private readonly WFCaseDefaults _defaults;
    private readonly IWFCaseLinkService _wFCaseLinkService;

    public WFCaselinkController(
        IWFCaseRepository repository,
        IBackOfficeClient backOfficeClient,
        ILogger<WFCaselinkController> logger,
        IConfiguration configuration,
        IOptions<WFCaseDefaults> defaults,
        IWFCaseLinkService wFCaseLinkService)
    {
        _repository = repository;
        _backOfficeClient = backOfficeClient;
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

        dto.Status = WFCaseLinkStatus.Created;

        //save in database
        WFCaseLink entity = dto.ToEntity();
        entity.Status = WFCaseLinkStatus.Created;
        await _repository.AddAsync(entity,ct);

        if (!Request.Headers.TryGetValue("X-Origin", out var origin) || origin != "FrontOffice")
        {
            try
            {
#if Sync
                //call API
                var output = await _backOfficeClient.SendLinkAsync(dto,ct);
                output.Status = WFCaseLinkStatus.Completed;
                await _repository.UpdateAsync(output.ToEntity(), ct);
#elif Async
                //this is the sending process
                await _wFCaseLinkService.SendToBackOffice(entity);
                //output.Status = WFCaseLinkStatus.Completed;
                //await _repository.UpdateAsync(output.ToEntity(), ct);
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending link to FrontOffice");
                //change the state to failed
                entity.Status = WFCaseLinkStatus.Failed;
                await _repository.UpdateWFStateToFailed(entity);
            }
        }
        return Ok(dto);
    }

    [HttpPost("callfo")]
    public async Task<IActionResult> CallFO(CancellationToken ct)
    {
        //filling mock data
        WFCaseLinkDto dto = new()
        {
           SourceCaseId = 5040,
           SourceMainEntityId = 456,
           SourceAppId = _defaults.SourceAppId,
           SourceMainEntityName = "Asset Registeration Office1",
           SourceWFClassName = "Asset Registering",
           CreatedAt = DateTime.UtcNow,
           TargetAppId = Guid.NewGuid(),
           TargetMainEntityName = "Government Registeration office",
           TargetWFClassName = "Government Asset Registering",
           CreatedByUserId = 111,
           LinkType = WFCaseLinkType.FOBO
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
}
