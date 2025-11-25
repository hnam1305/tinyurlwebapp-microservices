using UrlShortener.Models;
namespace UrlShortener.Services
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(string username, string email, string password);
        Task<User?> AuthenticateAsync(string username, string password);
    }
}