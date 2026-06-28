using Microsoft.AspNetCore.Components;
using TMS.Core.DTOs;
using TMS.Web.Services;
using AntDesign;

namespace TMS.Web.Components.Pages;

public partial class ReviewerDashboard : ComponentBase
{
    [Inject] private IQualityScoreService QualityScoreService { get; set; } = default!;
    [Inject] private MessageService Message { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await LoadScores();
    }

    private async Task LoadScores()
    {
        loading = true;
        try
        {
            scores = await QualityScoreService.GetAllAsync();
            CalculateRankings();
            CalculateDistribution();
        }
        catch (Exception ex)
        {
            Message.Error($"Failed to load quality scores: {ex.Message}");
        }
        finally
        {
            loading = false;
        }
    }

    private void CalculateRankings()
    {
        translatorRankings = scores
            .GroupBy(s => s.TranslatorName)
            .Select(g => new TranslatorRanking
            {
                TranslatorName = g.Key,
                ReviewCount = g.Count(),
                AverageQuality = g.Average(s => s.QualityPercentage),
                TotalErrors = g.Sum(s => s.ErrorCount)
            })
            .OrderByDescending(r => r.AverageQuality)
            .ThenByDescending(r => r.ReviewCount)
            .ToList();
    }

    private void CalculateDistribution()
    {
        excellentCount = scores.Count(s => s.QualityPercentage >= 95 && s.QualityPercentage <= 100);
        goodCount = scores.Count(s => s.QualityPercentage >= 80 && s.QualityPercentage < 95);
        needsImprovementCount = scores.Count(s => s.QualityPercentage < 80);
    }
}
