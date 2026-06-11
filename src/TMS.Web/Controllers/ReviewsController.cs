using Microsoft.AspNetCore.Mvc;
using TMS.Application.Services;

namespace TMS.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IApprovalService _approvalService;
    private readonly ILogger<ReviewsController> _logger;

    public ReviewsController(IApprovalService approvalService, ILogger<ReviewsController> logger)
    {
        _approvalService = approvalService;
        _logger = logger;
    }

    [HttpGet("task/{taskId}")]
    public async Task<IActionResult> GetByTaskId(string taskId)
    {
        var reviews = await _approvalService.GetByTaskIdAsync(taskId);
        return Ok(reviews);
    }

    [HttpGet("reviewer/{reviewerId}")]
    public async Task<IActionResult> GetByReviewerId(string reviewerId)
    {
        var reviews = await _approvalService.GetByReviewerIdAsync(reviewerId);
        return Ok(reviews);
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending()
    {
        var reviews = await _approvalService.GetPendingReviewsAsync();
        return Ok(reviews);
    }
}
