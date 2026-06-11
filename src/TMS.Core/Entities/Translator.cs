using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TMS.Core.Entities;

public class Translator
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = null!;

    public List<string> Languages { get; set; } = new();

    public string Specialization { get; set; } = string.Empty;

    public decimal RatePerWord { get; set; }

    public double AverageSpeed { get; set; }

    public double Rating { get; set; }
}
