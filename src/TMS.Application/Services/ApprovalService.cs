using Microsoft.Extensions.Logging;
using TMS.Core.Entities;
using TMS.Core.Enums;
using TMS.Core.Interfaces;

namespace TMS.Application.Services;

public interface IApprovalService
{
    Task<IEnumerable<Review>> GetByTaskIdAsync(string taskId);
    Task<IEnumerable<Review>> GetByReviewerIdAsync(string reviewerId);
    Task<IEnumerable<Review>> GetPendingReviewsAsync();
}

public class ApprovalService : IApprovalService
{
    private readonly IRepository<Review> _reviewRepository;
    private readonly ILogger<ApprovalService> _logger;

    public ApprovalService(IRepository<Review> reviewRepository, ILogger<ApprovalService> logger)
    {
        _reviewRepository = reviewRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<Review>> GetByTaskIdAsync(string taskId)
    {
        return await _reviewRepository.FindAsync(r => r.TaskId == taskId);
    }

    public async Task<IEnumerable<Review>> GetByReviewerIdAsync(string reviewerId)
    {
        return await _reviewRepository.FindAsync(r => r.ReviewerId == reviewerId);
    }

    public async Task<IEnumerable<Review>> GetPendingReviewsAsync()
    {
        return await _reviewRepository.FindAsync(r => r.Status == ApprovalStatus.Pending);
    }
}
