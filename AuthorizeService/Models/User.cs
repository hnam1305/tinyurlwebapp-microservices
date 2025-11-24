
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
namespace UrlShortener.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;
        public string Username { get; set; } = string.Empty;
        public String PasswordHash { get; set; } = String.Empty;
        public string Role { get; set; } = "User";
        public string? Email { get; set; }

        // reset mk
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpire { get; set; }

       
       
    }
}
