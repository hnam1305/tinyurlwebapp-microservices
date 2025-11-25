using MongoDB.Driver;
using UrlShortener.Data;
using UrlShortener.Models;
using System.Net.Mail;
using System.Net;


namespace UrlShortener.Services
{
    public class AuthService : IAuthService
    {
        private readonly MongoDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(MongoDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        
        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var user = await _context.Users.Find(u => u.Username == username).FirstOrDefaultAsync();
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;
            return user;
        }

        
        public async Task<bool> RegisterAsync(string username, string email, string password)
        {
            
            var exists = await _context.Users.Find(u => u.Username == username || u.Email == email).FirstOrDefaultAsync();
            if (exists != null) return false;

            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            var newUser = new User
            {
                Username = username,
                Email = email,
                PasswordHash = hash,
                Role = "User",
                ResetToken = null,
                ResetTokenExpire = null
            };

            await _context.Users.InsertOneAsync(newUser);
            return true;
        }

        

        

        
    }
}
