namespace TrelloClone.Tests.UnitTests;

public class UserServiceTests
{
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly Mock<IWorkspaceRepository> _workspaceRepoMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userManagerMock = new Mock<UserManager<AppUser>>(
            Mock.Of<IUserStore<AppUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);

        _workspaceRepoMock = new Mock<IWorkspaceRepository>();
        _userService = new UserService(_userManagerMock.Object, _workspaceRepoMock.Object);
    }

    [Fact]
    public async Task GetProfileAsync_WithExistingUser_ReturnsProfile()
    {
        var userId = Guid.NewGuid().ToString();
        var user = new AppUser
        {
            Id = userId,
            UserName = "testuser",
            Email = "test@test.com",
            DisplayName = "Test User",
            AvatarUrl = "avatar.jpg",
            IsEmailVerified = true,
            CreatedAt = DateTime.UtcNow
        };

        _userManagerMock.Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync(user);

        var result = await _userService.GetProfileAsync(userId);

        result.Id.Should().Be(userId);
        result.UserName.Should().Be("testuser");
        result.Email.Should().Be("test@test.com");
        result.DisplayName.Should().Be("Test User");
    }

    [Fact]
    public async Task GetProfileAsync_WithNonExistingUser_ThrowsKeyNotFoundException()
    {
        _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((AppUser?)null);

        await FluentActions.Invoking(() => _userService.GetProfileAsync("invalid-id"))
            .Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task UpdateProfileAsync_UpdatesDisplayName()
    {
        var userId = Guid.NewGuid().ToString();
        var user = new AppUser
        {
            Id = userId,
            UserName = "testuser",
            Email = "test@test.com"
        };

        _userManagerMock.Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync(user);

        _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(IdentityResult.Success);

        var request = new UpdateUserProfileRequest(
            DisplayName: "New Name",
            UserName: null,
            Email: null);

        var result = await _userService.UpdateProfileAsync(userId, request);

        result.DisplayName.Should().Be("New Name");
        user.DisplayName.Should().Be("New Name");
    }

    [Fact]
    public async Task UpdateProfileAsync_WithDuplicateEmail_ThrowsInvalidOperationException()
    {
        var userId = Guid.NewGuid().ToString();
        var user = new AppUser { Id = userId, UserName = "testuser", Email = "test@test.com" };
        var existingUser = new AppUser { Id = "other-id", UserName = "other", Email = "taken@test.com" };

        _userManagerMock.Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync(user);

        _userManagerMock.Setup(x => x.FindByEmailAsync("taken@test.com"))
            .ReturnsAsync(existingUser);

        var request = new UpdateUserProfileRequest(
            DisplayName: null,
            UserName: null,
            Email: "taken@test.com");

        await FluentActions.Invoking(() => _userService.UpdateProfileAsync(userId, request))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Email is already in use");
    }

    [Fact]
    public async Task UpdateAvatarAsync_SetsAvatarUrl()
    {
        var userId = Guid.NewGuid().ToString();
        var user = new AppUser { Id = userId };

        _userManagerMock.Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync(user);

        _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(IdentityResult.Success);

        var request = new UpdateAvatarRequest("https://example.com/avatar.jpg");

        var result = await _userService.UpdateAvatarAsync(userId, request);

        result.AvatarUrl.Should().Be("https://example.com/avatar.jpg");
        user.AvatarUrl.Should().Be("https://example.com/avatar.jpg");
    }

    [Fact]
    public async Task DeleteAccountAsync_SetsUserInactive()
    {
        var userId = Guid.NewGuid().ToString();
        var user = new AppUser { Id = userId, IsActive = true };

        _userManagerMock.Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync(user);

        _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(IdentityResult.Success);

        await _userService.DeleteAccountAsync(userId);

        user.IsActive.Should().BeFalse();
    }
}
