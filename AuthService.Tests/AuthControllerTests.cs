using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

using UrlShortener.Controllers;
using UrlShortener.Models;
using UrlShortener.Services;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authService;
    private readonly Mock<IJwtService> _jwtService;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _authService = new Mock<IAuthService>();
        _jwtService = new Mock<IJwtService>();

        _controller = new AuthController(_authService.Object, _jwtService.Object);
    }

    //test register

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenFieldsMissing()
    {
        var req = new RegisterRequest
        {
            Username = "",
            Email = "",
            Password = ""
        };

        var result = await _controller.Register(req);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenUserExists()
    {
        var req = new RegisterRequest
        {
            Username = "test",
            Email = "test@mail.com",
            Password = "123"
        };

        _authService.Setup(a => a.RegisterAsync(req.Username, req.Email, req.Password))
                    .ReturnsAsync(false);

        var result = await _controller.Register(req);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Register_ReturnsOk_WhenSuccess()
    {
        var req = new RegisterRequest
        {
            Username = "test",
            Email = "test@mail.com",
            Password = "123"
        };

        _authService.Setup(a => a.RegisterAsync(req.Username, req.Email, req.Password))
                    .ReturnsAsync(true);

        var result = await _controller.Register(req);

        Assert.IsType<OkObjectResult>(result);
    }

    //test login

    [Fact]
    public async Task Login_ReturnsBadRequest_WhenMissingFields()
    {
        var req = new LoginRequest
        {
            Username = "",
            Password = ""
        };

        var result = await _controller.Login(req);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenInvalidCredentials()
    {
        var req = new LoginRequest
        {
            Username = "test",
            Password = "wrong"
        };

        _authService.Setup(a => a.AuthenticateAsync(req.Username, req.Password))
                    .ReturnsAsync((User)null);

        var result = await _controller.Login(req);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Login_ReturnsOk_WhenValid()
    {
        var user = new User
        {
            Username = "test",
            Email = "test@mail.com",
            Role = "User"
        };

        var req = new LoginRequest
        {
            Username = "test",
            Password = "123"
        };

        _authService.Setup(a => a.AuthenticateAsync(req.Username, req.Password))
                    .ReturnsAsync(user);

        _jwtService.Setup(j => j.GenerateToken(user))
                   .Returns("fakeToken123");

        var result = await _controller.Login(req);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }

    //test logout
    [Fact]
    public void Logout_ReturnsOk()
    {
        var controller = new AuthController(_authService.Object, _jwtService.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        var result = controller.Logout();

        Assert.IsType<OkObjectResult>(result);
    }
}
