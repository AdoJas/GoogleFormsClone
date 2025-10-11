using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GoogleFormsClone.Models
{
    public class RefreshToken
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("token")]
        public string Token { get; set; } = string.Empty;

        [BsonElement("expiresAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime ExpiresAt { get; set; }

        [BsonElement("isRevoked")]
        public bool IsRevoked { get; set; } = false;
    }
}