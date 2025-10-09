using MongoDB.Bson.Serialization.Attributes;

namespace GoogleFormsClone.Models
{
    public class RefreshToken
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = null!;
        
        public string UserId { get; set; } = null!;
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; } = false;
    }
}