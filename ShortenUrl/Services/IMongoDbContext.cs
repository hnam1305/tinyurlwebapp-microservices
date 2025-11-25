using MongoDB.Driver;
using ShortenUrl.Models;

namespace ShortenUrl.Services
{
    public interface IMongoDbContext
    {
        IMongoCollection<ShortUrl> ShortUrls { get; }
    }
}
