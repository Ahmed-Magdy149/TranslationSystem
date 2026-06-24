using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TMS.Core.Entities;

public class ServiceRate
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    public string ServiceType { get; set; } = null!;

    public string Role { get; set; } = null!;

    public int WordsPerHour { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
