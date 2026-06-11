using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TMS.Core.Entities;

public class TranslationTask
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Description { get; set; } = string.Empty;

    public string SourceLanguage { get; set; } = null!;

    public string TargetLanguage { get; set; } = null!;

    public string FileName { get; set; } = string.Empty;

    public string FileUrl { get; set; } = string.Empty;

    public int WordCount { get; set; }

    public double EstimatedHours { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? AssignedTranslatorId { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string CreatedBy { get; set; } = null!;

    [BsonRepresentation(BsonType.String)]
    public Enums.TaskStatus Status { get; set; } = Enums.TaskStatus.Pending;

    public DateTime? Deadline { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
