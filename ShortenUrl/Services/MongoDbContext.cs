using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using ShortenUrl.Controllers;
using ShortenUrl.Models;
namespace ShortenUrl.Services
{
    public class MongoDbContext
    {
        public IMongoCollection<ShortUrl> ShortUrls { get; }

        public MongoDbContext(IConfiguration config)
        {
            var client = new MongoClient(config["Mongo:ConnectionString"]);
            var db = client.GetDatabase(config["Mongo:Database"]);

            ShortUrls = db.GetCollection<ShortUrl>("short_urls");
        }
    }
}

