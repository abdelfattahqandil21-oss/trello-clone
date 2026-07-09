using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;

namespace TrelloClone.Tests.IntegrationTests;

public class AuthApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AuthApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ReturnsSuccess()
    {
        var body = new
        {
            email = $"test_{Guid.NewGuid():N}@test.com",
            password = "Test123!",
            confirmPassword = "Test123!",
            username = $"user_{Guid.NewGuid():N}"
        };

        var json = JsonConvert.SerializeObject(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/auth/register", content);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseBody = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<AuthResponse>(responseBody);

        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsBadRequest()
    {
        var email = $"dup_{Guid.NewGuid():N}@test.com";

        var body1 = new
        {
            email,
            password = "Test123!",
            confirmPassword = "Test123!",
            username = $"user1_{Guid.NewGuid():N}"
        };

        var body2 = new
        {
            email,
            password = "Test123!",
            confirmPassword = "Test123!",
            username = $"user2_{Guid.NewGuid():N}"
        };

        var json1 = JsonConvert.SerializeObject(body1);
        var json2 = JsonConvert.SerializeObject(body2);

        await _client.PostAsync("/api/auth/register",
            new StringContent(json1, Encoding.UTF8, "application/json"));

        var response2 = await _client.PostAsync("/api/auth/register",
            new StringContent(json2, Encoding.UTF8, "application/json"));

        response2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        var body = new { email = "nonexistent@test.com", password = "WrongPass1!" };
        var json = JsonConvert.SerializeObject(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/auth/login", content);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUsersMe_WithoutAuth_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/users/me");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SearchUsers_WithoutAuth_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/users/search?q=test");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
