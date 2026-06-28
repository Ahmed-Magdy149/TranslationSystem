using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TMS.Core.Entities;

public class QualityScoreRecord
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime Date { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string TranslatorId { get; set; } = null!;

    public string TranslatorName { get; set; } = null!;

    [BsonRepresentation(BsonType.ObjectId)]
    public string ReviewerId { get; set; } = null!;

    public string ReviewerName { get; set; } = null!;

    public string ProjectName { get; set; } = string.Empty;

    public string ServiceType { get; set; } = null!;

    public int ReviewedWords { get; set; }

    public double ReviewHours { get; set; }

    /// <summary>
    /// Quality score from 0-100 (max score)
    /// </summary>
    public double MaximumScore { get; set; } = 100;

    /// <summary>
    /// Quality score achieved by translator (0-100)
    /// </summary>
    public double QualityScore { get; set; }

    /// <summary>
    /// Automatically calculated: (QualityScore / MaximumScore) * 100
    /// </summary>
    public double QualityPercentage { get; set; }

    public int MajorErrors { get; set; }

    public int MinorErrors { get; set; }

    public int ErrorCount => MajorErrors + MinorErrors;

    public string Comments { get; set; } = string.Empty;

    /// <summary>
    /// Calculated performance status: Excellent / Good / Needs Improvement
    /// </summary>
    public string PerformanceStatus { get; set; } = string.Empty;

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? UpdatedAt { get; set; }
}
