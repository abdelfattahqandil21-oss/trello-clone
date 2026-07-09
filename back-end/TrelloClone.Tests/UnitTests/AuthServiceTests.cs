using Microsoft.Extensions.Configuration;

namespace TrelloClone.Tests.UnitTests;

public class AuthServiceTests
{
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly Mock<AppDpContext> _contextMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userManagerMock = new Mock<UserManager<AppUser>>(
            Mock.Of<IUserStore<AppUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);

        _contextMock = new Mock<AppDpContext>(new DbContextOptions<AppDpContext>());

        _configMock = new Mock<IConfiguration>();
        var jwtSectionMock = new Mock<IConfigurationSection>();
        jwtSectionMock.Setup(x => x["Key"]).Returns("TestKeyThatIsAtLeast32CharactersLong!@#$%^&*");
        jwtSectionMock.Setup(x => x["Issuer"]).Returns("TestIssuer");
        jwtSectionMock.Setup(x => x["Audience"]).Returns("TestAudience");
        _configMock.Setup(x => x.GetSection("Jwt")).Returns(jwtSectionMock.Object);

        _loggerMock = new Mock<ILogger<AuthService>>();

        _authService = new AuthService(
            _userManagerMock.Object,
            _contextMock.Object,
            _configMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_ReturnsSuccess()
    {
        var request = new RegisterRequest
        {
            Email = "test@test.com",
            Password = "Test123!",
            ConfirmPassword = "Test123!",
            Username = "testuser"
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email))
            .ReturnsAsync((AppUser?)null);

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), request.Password))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<AppUser>()))
            .ReturnsAsync("email-token");

        var result = await _authService.RegisterAsync(request);

        result.Success.Should().BeTrue();
        result.Message.Should().Be("Registration successful. Please verify your email.");
    }

    [Fact]
    public async Task RegisterAsync_WithDuplicateEmail_ReturnsFail()
    {
        var request = new RegisterRequest
        {
            Email = "existing@test.com",
            Password = "Test123!",
            ConfirmPassword = "Test123!",
            Username = "testuser"
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email))
            .ReturnsAsync(new AppUser { Email = request.Email });

        var result = await _authService.RegisterAsync(request);

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Email is already registered.");
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsAccessToken()
    {
        var request = new LoginRequest
        {
            Email = "test@test.com",
            Password = "Test123!"
        };

        var user = new AppUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = request.Email,
            UserName = "testuser",
            IsActive = true,
            IsEmailVerified = true
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email))
            .ReturnsAsync(user);

        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, request.Password))
            .ReturnsAsync(true);

        _userManagerMock.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new List<string>());

        var refreshTokenMock = new Mock<DbSet<RefreshToken>>();
        _contextMock.Setup(x => x.RefreshTokens).Returns(refreshTokenMock.Object);
        _contextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _authService.LoginAsync(request);

        result.Success.Should().BeTrue();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ReturnsFail()
    {
        var request = new LoginRequest
        {
            Email = "test@test.com",
            Password = "WrongPassword!"
        };

        var user = new AppUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = request.Email,
            UserName = "testuser",
            IsActive = true,
            IsEmailVerified = true
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email))
            .ReturnsAsync(user);

        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, request.Password))
            .ReturnsAsync(false);

        var result = await _authService.LoginAsync(request);

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Invalid email or password.");
    }

    [Fact]
    public async Task LoginAsync_WithUnverifiedEmail_ReturnsFail()
    {
        var request = new LoginRequest
        {
            Email = "test@test.com",
            Password = "Test123!"
        };

        var user = new AppUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = request.Email,
            UserName = "testuser",
            IsActive = true,
            IsEmailVerified = false
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email))
            .ReturnsAsync(user);

        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, request.Password))
            .ReturnsAsync(true);

        var result = await _authService.LoginAsync(request);

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Email not verified. Please verify your email first.");
    }

    [Fact]
    public async Task LoginAsync_WithInactiveAccount_ReturnsFail()
    {
        var request = new LoginRequest
        {
            Email = "test@test.com",
            Password = "Test123!"
        };

        var user = new AppUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = request.Email,
            UserName = "testuser",
            IsActive = false,
            IsEmailVerified = true
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email))
            .ReturnsAsync(user);

        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, request.Password))
            .ReturnsAsync(true);

        var result = await _authService.LoginAsync(request);

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Account is deactivated.");
    }

    [Fact]
    public async Task LogoutAsync_RevokesAllRefreshTokens()
    {
        var userId = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<AppDpContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new AppDpContext(options);
        context.RefreshTokens.Add(new RefreshToken
        {
            UserId = userId,
            Token = "token1",
            IsUsed = false,
            IsRevoked = false
        });
        await context.SaveChangesAsync();

        var authService = new AuthService(_userManagerMock.Object, context, _configMock.Object, _loggerMock.Object);

        var result = await authService.LogoutAsync(userId);

        result.Success.Should().BeTrue();
        result.Message.Should().Be("Logged out successfully.");
    }
}
