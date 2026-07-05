using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Btr.IntegrationTests;

public class HealthEndpointTests : IClassFixture<BtrWebApplicationFactory>
{
    private readonly BtrWebApplicationFactory _factory;

    public HealthEndpointTests(BtrWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_Health_ReturnsOk()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Post_Register_WithValidPayload_ReturnsCreated()
    {
        using var client = _factory.CreateClient();
        var email = $"user-{Guid.NewGuid():N}@test.local";

        var response = await client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            UsernameOrEmail = email,
            Password = "P@ssw0rd123"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Post_Register_WithDuplicateUser_ReturnsBadRequest()
    {
        using var client = _factory.CreateClient();
        var email = $"dup-{Guid.NewGuid():N}@test.local";

        await client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            UsernameOrEmail = email,
            Password = "P@ssw0rd123"
        });

        var duplicateResponse = await client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            UsernameOrEmail = email,
            Password = "P@ssw0rd123"
        });

        Assert.Equal(HttpStatusCode.BadRequest, duplicateResponse.StatusCode);
    }

    [Fact]
    public async Task Post_Login_WithValidCredentials_ReturnsOkAndToken()
    {
        using var client = _factory.CreateClient();
        var email = $"login-{Guid.NewGuid():N}@test.local";
        const string password = "P@ssw0rd123";

        await client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            UsernameOrEmail = email,
            Password = password
        });

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            UsernameOrEmail = email,
            Password = password
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(payload);
        var token = json.RootElement.GetProperty("accessToken").GetString();
        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public async Task Post_Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient();
        var email = $"invalid-login-{Guid.NewGuid():N}@test.local";

        await client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            UsernameOrEmail = email,
            Password = "P@ssw0rd123"
        });

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            UsernameOrEmail = email,
            Password = "wrong-password"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Post_Registrations_WithoutToken_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/registrations", new
        {
            TournamentId = Guid.NewGuid(),
            PlayerName = "Test Player",
            ContactInfo = "test@example.com"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Put_Registrations_WithoutToken_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient();

        var response = await client.PutAsJsonAsync($"/api/v1/registrations/{Guid.NewGuid():N}", new
        {
            PlayerName = "Updated Player"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Registrations_WithoutToken_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient();

        var response = await client.DeleteAsync($"/api/v1/registrations/{Guid.NewGuid():N}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
