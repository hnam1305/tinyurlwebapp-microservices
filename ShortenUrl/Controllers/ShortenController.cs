using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ShortenUrl.Models;
using ShortenUrl.Services;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace ShortenUrl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShortenController : ControllerBase
    {
        private readonly IShortUrlRepository _repo;
        private readonly IConfiguration _config;

        public ShortenController(IShortUrlRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        public class CreateRequest
        {
            public string Url { get; set; } = "";
            public string? Alias { get; set; }
        }
        public class EditRequest
        {
            public string OldId { get; set; }
            public string NewId { get; set; }
        }

        private string GenerateRandomId()
        {
            string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random r = new Random();
            string output = "";

            for (int i = 0; i < 6; i++)
            {
                output += chars[r.Next(chars.Length)];
            }

            return output;
        }

        private string? GetUserId()
        {
            if (User?.Identity == null || !User.Identity.IsAuthenticated)
                return null;

            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null) return claim.Value;

            claim = User.FindFirst(JwtRegisteredClaimNames.Sub);
            if (claim != null) return claim.Value;

            claim = User.FindFirst("sub");
            if (claim != null) return claim.Value;

            claim = User.FindFirst("nameid");
            if (claim != null) return claim.Value;

            return null;
        }

        [AllowAnonymous]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Url))
                return BadRequest("Empty URL");

            if (!Uri.IsWellFormedUriString(req.Url, UriKind.Absolute))
                return BadRequest("Invalid URL format");

            string id;

            // ALIAS CASE
            if (!string.IsNullOrEmpty(req.Alias))
            {
                if (req.Alias.Length > 5)
                    return BadRequest("Alias cannot be longer than 5 characters.");

                id = req.Alias;

                var exists = await _repo.GetAlias(id);
                if (exists != null)
                    return Conflict("Alias already exists.");
            }
            else
            {
                // RANDOM ID LOOP
                do
                {
                    id = GenerateRandomId();
                    var exists = await _repo.GetAlias(id);
                    if (exists == null) break;
                } while (true);
            }

            var userId = GetUserId();

            var item = new ShortUrl
            {
                ShortId = id,
                OriginalUrl = req.Url,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.InsertAsync(item);

            string baseUrl = _config["AppConfig:PublicBaseUrl"] ?? $"{Request.Scheme}://{Request.Host}";
            string generated = $"{baseUrl}/{id}";

            return Ok(new
            {
                shortId = id,
                originalUrl = req.Url,
                shortUrl = generated
            });
        }

        [Authorize]
        [HttpGet("history")]
        public async Task<IActionResult> History()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not logged in.");

            var items = await _repo.GetUserHistory(userId);

            string baseUrl = _config["AppConfig:PublicBaseUrl"] ?? $"{Request.Scheme}://{Request.Host}";
            var result = items.Select(x => new
            {
                x.ShortId,
                x.OriginalUrl,
                shortUrl = $"{baseUrl}/{x.ShortId}",
                x.CreatedAt
            });

            return Ok(result);
        }

        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not logged in.");

            var success = await _repo.DeleteUserShortUrl(id, userId);

            if (!success)
                return NotFound("Short URL not found or not owned by user.");

            return Ok("Deleted successfully.");
        }

        [Authorize]
        [HttpPost("edit")]
        public async Task<IActionResult> Edit([FromBody] EditRequest req)
        {
            var userId = GetUserId();

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not logged in.");

            if (string.IsNullOrWhiteSpace(req.NewId))
                return BadRequest("Invalid new url's id");

            if (req.NewId.Length > 5)
                return BadRequest("Id cannot be longer than 5");

            var exist = await _repo.GetAlias(req.NewId);
            if (exist != null)
                return BadRequest("Id already exist");

            var updated = await _repo.UpdateShortId(req.OldId, req.NewId, userId);
            if (!updated)
                return BadRequest("This url is not available for this user");

            return Ok("Updated");
        }

        [AllowAnonymous]
        [HttpGet("~/{id}")]
        public async Task<IActionResult> RedirectToUrl(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var item = await _repo.GetByShortId(id);

            if (item == null)
                return NotFound($"Short URL with ID '{id}' not found.");

            return Redirect(item.OriginalUrl);
        }
    }
}
