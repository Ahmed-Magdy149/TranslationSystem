using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TMS.Core.Entities;

public class TaskHistory
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    public string TaskId { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public string Action { get; set; } = null!;

    public string Comment { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.String)]
    public Enums.TaskStatus? OldStatus { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Enums.TaskStatus? NewStatus { get; set; }

    public string? DelayReason { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
