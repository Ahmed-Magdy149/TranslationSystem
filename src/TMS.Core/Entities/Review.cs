using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TMS.Core.Enums;

namespace TMS.Core.Entities;

public class Review
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonRepresentation(BsonType.ObjectId)]
    public string TaskId { get; set; } = null!;

    [BsonRepresentation(BsonType.ObjectId)]
    public string ReviewerId { get; set; } = null!;

    [BsonRepresentation(BsonType.String)]
    public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;

    public string Notes { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
