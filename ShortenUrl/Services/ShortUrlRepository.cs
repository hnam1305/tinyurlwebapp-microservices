using MongoDB.Driver;
using ShortenUrl.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShortenUrl.Services
{
    public class ShortUrlRepository : IShortUrlRepository
    {
        private readonly IMongoDbContext _db;

        public ShortUrlRepository(IMongoDbContext db)
        {
            _db = db;
        }

        public async Task<ShortUrl?> GetByShortId(string id)
        {
            return await _db.ShortUrls.Find(x => x.ShortId == id)
                                      .FirstOrDefaultAsync();
        }

        public async Task<ShortUrl?> GetAlias(string alias)
        {
            return await _db.ShortUrls.Find(x => x.ShortId == alias)
                                      .FirstOrDefaultAsync();
        }

        public async Task InsertAsync(ShortUrl item)
        {
            await _db.ShortUrls.InsertOneAsync(item);
        }

        public async Task<List<ShortUrl>> GetUserHistory(string userId)
        {
            return await _db.ShortUrls
                .Find(x => x.UserId == userId)
                .SortByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> DeleteUserShortUrl(string id, string userId)
        {
            var r = await _db.ShortUrls.DeleteOneAsync(
                x => x.ShortId == id && x.UserId == userId);
            return r.DeletedCount > 0;
        }

        public async Task<bool> UpdateShortId(string oldId, string newId, string userId)
        {
            var item = await _db.ShortUrls.Find(x => x.ShortId == oldId && x.UserId == userId)
                                          .FirstOrDefaultAsync();
            if (item == null) return false;

            item.ShortId = newId;
            await _db.ShortUrls.ReplaceOneAsync(x => x.Id == item.Id, item);
            return true;
        }
    }
}
