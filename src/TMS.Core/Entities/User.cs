using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TMS.Core.Enums;

namespace TMS.Core.Entities;

[BsonIgnoreExtraElements]
public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("Name")]
    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = null!;

    [BsonRepresentation(BsonType.String)]
    public UserRole Role { get; set; }

    public string Language { get; set; } = string.Empty;

    public string Specialization { get; set; } = string.Empty;

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime JoiningDate { get; set; } = DateTime.UtcNow;

    [BsonRepresentation(BsonType.String)]
    public UserStatus Status { get; set; } = UserStatus.Active;

    public int DailyTarget { get; set; } = 0;

    public double HourlyRate { get; set; } = 0;

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? LastLoginDate { get; set; }

    public string MainService { get; set; } = string.Empty;

    public string ExperienceLevel { get; set; } = string.Empty;
}
