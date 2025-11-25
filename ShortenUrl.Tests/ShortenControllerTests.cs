using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using ShortenUrl.Controllers;
using ShortenUrl.Models;
using ShortenUrl.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;

public class ShortenControllerTests
{
    private readonly Mock<IShortUrlRepository> _repo;
    private readonly Mock<IConfiguration> _config;

    public ShortenControllerTests()
    {
        _repo = new Mock<IShortUrlRepository>();

        _config = new Mock<IConfiguration>();
        _config.Setup(c => c["AppConfig:PublicBaseUrl"])
               .Returns("https://test.com");
    }

    private ShortenController CreateControllerWithUser(string userId)
    {
        var controller = new ShortenController(_repo.Object, _config.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.NameIdentifier, userId) },
            "mock"));

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        return controller;
    }

    //test create

    [Fact]
    public async Task Create_ReturnsOk_WhenUrlIsValid()
    {
        var controller = new ShortenController(_repo.Object, _config.Object);

        var req = new ShortenController.CreateRequest { Url = "https://google.com" };
        _repo.Setup(r => r.GetAlias(It.IsAny<string>())).ReturnsAsync((ShortUrl)null);

        var result = await controller.Create(req);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenUrlInvalid()
    {
        var controller = new ShortenController(_repo.Object, _config.Object);
        var req = new ShortenController.CreateRequest { Url = "bad-url" };

        var result = await controller.Create(req);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsConflict_WhenAliasExists()
    {
        var controller = new ShortenController(_repo.Object, _config.Object);
        var req = new ShortenController.CreateRequest { Url = "https://google.com", Alias = "abc" };

        _repo.Setup(r => r.GetAlias("abc")).ReturnsAsync(new ShortUrl());

        var result = await controller.Create(req);

        Assert.IsType<ConflictObjectResult>(result);
    }

    //test redirect

    [Fact]
    public async Task RedirectToUrl_ReturnsNotFound_WhenIdNotFound()
    {
        var controller = new ShortenController(_repo.Object, _config.Object);
        _repo.Setup(r => r.GetByShortId("abc")).ReturnsAsync((ShortUrl)null);

        var result = await controller.RedirectToUrl("abc");

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task RedirectToUrl_ReturnsRedirect_WhenIdExists()
    {
        var controller = new ShortenController(_repo.Object, _config.Object);

        _repo.Setup(r => r.GetByShortId("abc"))
             .ReturnsAsync(new ShortUrl { ShortId = "abc", OriginalUrl = "https://google.com" });

        var result = await controller.RedirectToUrl("abc");

        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Equal("https://google.com", redirect.Url);
    }

    //test history

    [Fact]
    public async Task History_ReturnsUnauthorized_WhenNoUser()
    {
        var controller = new ShortenController(_repo.Object, _config.Object);

        var result = await controller.History();

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task History_ReturnsList_WhenLoggedIn()
    {
        var controller = CreateControllerWithUser("u1");

        _repo.Setup(r => r.GetUserHistory("u1"))
             .ReturnsAsync(new List<ShortUrl>
             {
                 new ShortUrl { ShortId = "abc", OriginalUrl = "https://google.com" }
             });

        var result = await controller.History();

        Assert.IsType<OkObjectResult>(result);
    }

   //test delete
    [Fact]
    public async Task Delete_ReturnsUnauthorized_WhenNoUser()
    {
        var controller = new ShortenController(_repo.Object, _config.Object);
        var result = await controller.Delete("abc");

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenNotOwned()
    {
        var controller = CreateControllerWithUser("user1");

        _repo.Setup(r => r.DeleteUserShortUrl("abc", "user1"))
             .ReturnsAsync(false);

        var result = await controller.Delete("abc");

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsOk_WhenDeleted()
    {
        var controller = CreateControllerWithUser("user1");

        _repo.Setup(r => r.DeleteUserShortUrl("abc", "user1"))
             .ReturnsAsync(true);

        var result = await controller.Delete("abc");

        Assert.IsType<OkObjectResult>(result);
    }

    //test edit

    [Fact]
    public async Task Edit_ReturnsBadRequest_WhenNewIdTooLong()
    {
        var controller = CreateControllerWithUser("user1");

        var req = new ShortenController.EditRequest { OldId = "a", NewId = "123456" };

        var result = await controller.Edit(req);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Edit_ReturnsBadRequest_WhenNewIdExists()
    {
        var controller = CreateControllerWithUser("user1");

        var req = new ShortenController.EditRequest { OldId = "a", NewId = "123" };

        _repo.Setup(r => r.GetAlias("123")).ReturnsAsync(new ShortUrl());

        var result = await controller.Edit(req);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Edit_ReturnsOk_WhenUpdated()
    {
        var controller = CreateControllerWithUser("user1");

        var req = new ShortenController.EditRequest { OldId = "a", NewId = "b" };

        _repo.Setup(r => r.GetAlias("b")).ReturnsAsync((ShortUrl)null);
        _repo.Setup(r => r.UpdateShortId("a", "b", "user1"))
             .ReturnsAsync(true);

        var result = await controller.Edit(req);

        Assert.IsType<OkObjectResult>(result);
    }
}
