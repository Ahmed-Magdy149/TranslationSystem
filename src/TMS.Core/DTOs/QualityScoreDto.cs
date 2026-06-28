namespace TMS.Core.DTOs;

public class QualityScoreDto
{
    public string Id { get; set; } = null!;
    public DateTime Date { get; set; }
    public string TranslatorId { get; set; } = null!;
    public string TranslatorName { get; set; } = null!;
    public string ReviewerId { get; set; } = null!;
    public string ReviewerName { get; set; } = null!;
    public string ProjectName { get; set; } = string.Empty;
    public string ServiceType { get; set; } = null!;
    public int ReviewedWords { get; set; }
    public double ReviewHours { get; set; }
    public double MaximumScore { get; set; } = 100;
    public double QualityScore { get; set; }
    public double QualityPercentage { get; set; }
    public int MajorErrors { get; set; }
    public int MinorErrors { get; set; }
    public int ErrorCount { get; set; }
    public string Comments { get; set; } = string.Empty;
    public string PerformanceStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateQualityScoreDto
{
    public DateTime Date { get; set; } = DateTime.Today;
    public string TranslatorId { get; set; } = null!;
    public string ReviewerId { get; set; } = null!;
    public string ProjectName { get; set; } = string.Empty;
    public string ServiceType { get; set; } = null!;
    public int ReviewedWords { get; set; }
    public double ReviewHours { get; set; }
    public double MaximumScore { get; set; } = 100;
    public double QualityScore { get; set; }
    public int MajorErrors { get; set; }
    public int MinorErrors { get; set; }
    public string Comments { get; set; } = string.Empty;
}

public class UpdateQualityScoreDto
{
    public DateTime Date { get; set; }
    public string TranslatorId { get; set; } = null!;
    public string ProjectName { get; set; } = string.Empty;
    public string ServiceType { get; set; } = null!;
    public int ReviewedWords { get; set; }
    public double ReviewHours { get; set; }
    public double MaximumScore { get; set; } = 100;
    public double QualityScore { get; set; }
    public int MajorErrors { get; set; }
    public int MinorErrors { get; set; }
    public string Comments { get; set; } = string.Empty;
}
