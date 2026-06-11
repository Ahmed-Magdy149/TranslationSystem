using Microsoft.AspNetCore.Mvc;
using TMS.Application.Interfaces;
using TMS.Core.DTOs;

namespace TMS.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TranslatorsController : ControllerBase
{
    private readonly ITranslatorService _translatorService;
    private readonly ILogger<TranslatorsController> _logger;

    public TranslatorsController(ITranslatorService translatorService, ILogger<TranslatorsController> logger)
    {
        _translatorService = translatorService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var translators = await _translatorService.GetAllAsync();
        return Ok(translators);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var translator = await _translatorService.GetByIdAsync(id);
        if (translator == null)
            return NotFound();

        return Ok(translator);
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailable()
    {
        var translators = await _translatorService.GetAvailableAsync();
        return Ok(translators);
    }

    [HttpGet("language/{language}")]
    public async Task<IActionResult> GetByLanguage(string language)
    {
        var translators = await _translatorService.GetByLanguageAsync(language);
        return Ok(translators);
    }

    [HttpGet("{id}/workload")]
    public async Task<IActionResult> GetWorkload(string id)
    {
        var workload = await _translatorService.CalculateWorkloadAsync(id);
        return Ok(new { TranslatorId = id, ActiveTasks = workload });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTranslatorDto dto)
    {
        try
        {
            var translator = await _translatorService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = translator.Id }, translator);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to create translator profile");
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateTranslatorDto dto)
    {
        try
        {
            await _translatorService.UpdateAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
