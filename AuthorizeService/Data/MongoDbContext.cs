using MongoDB.Driver;
using UrlShortener.Models;

namespace UrlShortener.Data
{
    public class MongoDbContext
    {
        public IMongoCollection<User> Users { get; }

        public MongoDbContext(IConfiguration config)
        {
            var client = new MongoClient(config["Mongo:ConnectionString"]);
            var db = client.GetDatabase(config["Mongo:Database"]);

            Users = db.GetCollection<User>("users");
        }
    }
}
