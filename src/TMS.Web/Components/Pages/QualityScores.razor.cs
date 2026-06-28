using Microsoft.AspNetCore.Components;
using TMS.Core.DTOs;
using TMS.Core.Enums;
using TMS.Web.Services;
using AntDesign;

namespace TMS.Web.Components.Pages;

public partial class QualityScores : ComponentBase
{
    [Inject] private IQualityScoreService QualityScoreService { get; set; } = default!;
    [Inject] private IUserService UserService { get; set; } = default!;
    [Inject] private MessageService Message { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await LoadScores();
        await LoadUsers();
    }

    private async Task LoadScores()
    {
        loading = true;
        try
        {
            scores = await QualityScoreService.GetAllAsync();
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

    private async Task LoadUsers()
    {
        try
        {
            var users = await UserService.GetAllUsersAsync();
            translators = users.Where(u => u.Role == UserRole.Translator || u.Role == UserRole.QAMember).ToList();
            reviewers = users.Where(u => u.Role == UserRole.Reviewer || u.Role == UserRole.Manager || u.Role == UserRole.Admin).ToList();
        }
        catch (Exception ex)
        {
            Message.Error($"Failed to load users: {ex.Message}");
        }
    }

    private void ShowAddModal()
    {
        isEditMode = false;
        currentScore = new CreateQualityScoreDto
        {
            Date = DateTime.Today,
            TranslatorId = string.Empty,
            ReviewerId = string.Empty,
            MaximumScore = 100,
            ServiceType = serviceTypes.FirstOrDefault() ?? "Translation"
        };
        editScoreId = string.Empty;
        modalVisible = true;
    }

    private void ShowEditModal(QualityScoreDto score)
    {
        isEditMode = true;
        editScoreId = score.Id;
        currentScore = new CreateQualityScoreDto
        {
            Date = score.Date,
            TranslatorId = score.TranslatorId,
            ReviewerId = score.ReviewerId,
            ProjectName = score.ProjectName,
            ServiceType = score.ServiceType,
            ReviewedWords = score.ReviewedWords,
            ReviewHours = score.ReviewHours,
            MaximumScore = score.MaximumScore,
            QualityScore = score.QualityScore,
            MajorErrors = score.MajorErrors,
            MinorErrors = score.MinorErrors,
            Comments = score.Comments
        };
        modalVisible = true;
    }

    private async Task HandleSubmit()
    {
        try
        {
            if (isEditMode)
            {
                var updateDto = new UpdateQualityScoreDto
                {
                    Date = currentScore.Date,
                    TranslatorId = currentScore.TranslatorId,
                    ProjectName = currentScore.ProjectName,
                    ServiceType = currentScore.ServiceType,
                    ReviewedWords = currentScore.ReviewedWords,
                    ReviewHours = currentScore.ReviewHours,
                    MaximumScore = currentScore.MaximumScore,
                    QualityScore = currentScore.QualityScore,
                    MajorErrors = currentScore.MajorErrors,
                    MinorErrors = currentScore.MinorErrors,
                    Comments = currentScore.Comments
                };
                var success = await QualityScoreService.UpdateAsync(editScoreId, updateDto);
                if (success)
                {
                    Message.Success("Quality score updated successfully!");
                    modalVisible = false;
                    await LoadScores();
                }
                else
                {
                    Message.Error("Failed to update quality score");
                }
            }
            else
            {
                var success = await QualityScoreService.CreateAsync(currentScore);
                if (success)
                {
                    Message.Success("Quality score created successfully!");
                    modalVisible = false;
                    await LoadScores();
                }
                else
                {
                    Message.Error("Failed to create quality score");
                }
            }
        }
        catch (Exception ex)
        {
            Message.Error($"Error: {ex.Message}");
        }
    }

    private void HandleCancel()
    {
        modalVisible = false;
    }

    private async Task DeleteScore(string scoreId)
    {
        try
        {
            var success = await QualityScoreService.DeleteAsync(scoreId);
            if (success)
            {
                Message.Success("Quality score deleted successfully!");
                await LoadScores();
            }
            else
            {
                Message.Error("Failed to delete quality score");
            }
        }
        catch (Exception ex)
        {
            Message.Error($"Error: {ex.Message}");
        }
    }
}
