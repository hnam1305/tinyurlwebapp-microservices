using UrlShortener.Models;

namespace UrlShortener.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
