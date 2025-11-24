

using MongoDB.Bson;
using System;

namespace ShortenUrl.Models
{
    public class ShortUrl
    {
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        public string ShortId { get; set; } = "";
        public string OriginalUrl { get; set; } = "";
        public string? UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
