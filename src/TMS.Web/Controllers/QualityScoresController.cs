using Microsoft.AspNetCore.Mvc;
using TMS.Application.Interfaces;
using TMS.Core.DTOs;

namespace TMS.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QualityScoresController : ControllerBase
{
    private readonly IQualityScoreService _qualityScoreService;
    private readonly ILogger<QualityScoresController> _logger;

    public QualityScoresController(IQualityScoreService qualityScoreService, ILogger<QualityScoresController> logger)
    {
        _qualityScoreService = qualityScoreService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var scores = await _qualityScoreService.GetAllAsync();
        return Ok(scores);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var score = await _qualityScoreService.GetByIdAsync(id);
        if (score == null)
            return NotFound();

        return Ok(score);
    }

    [HttpGet("translator/{translatorId}")]
    public async Task<IActionResult> GetByTranslator(string translatorId)
    {
        var scores = await _qualityScoreService.GetByTranslatorIdAsync(translatorId);
        return Ok(scores);
    }

    [HttpGet("reviewer/{reviewerId}")]
    public async Task<IActionResult> GetByReviewer(string reviewerId)
    {
        var scores = await _qualityScoreService.GetByReviewerIdAsync(reviewerId);
        return Ok(scores);
    }

    [HttpGet("range")]
    public async Task<IActionResult> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var scores = await _qualityScoreService.GetByDateRangeAsync(startDate, endDate);
        return Ok(scores);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateQualityScoreDto dto)
    {
        try
        {
            var score = await _qualityScoreService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = score.Id }, score);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Failed to create quality score");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating quality score");
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateQualityScoreDto dto)
    {
        try
        {
            await _qualityScoreService.UpdateAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating quality score");
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            await _qualityScoreService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting quality score");
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
