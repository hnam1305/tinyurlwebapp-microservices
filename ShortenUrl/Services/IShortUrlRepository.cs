using ShortenUrl.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ShortenUrl.Services
{
    public interface IShortUrlRepository
    {
        Task<ShortUrl?> GetByShortId(string id);
        Task<ShortUrl?> GetAlias(string alias);
        Task InsertAsync(ShortUrl item);
        Task<List<ShortUrl>> GetUserHistory(string userId);
        Task<bool> DeleteUserShortUrl(string id, string userId);
        Task<bool> UpdateShortId(string oldId, string newId, string userId);
    }
}
