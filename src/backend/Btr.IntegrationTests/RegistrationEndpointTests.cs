using System.Net;
using System.Net.Http.Json;
using Btr.Application.Features.Tournaments.Create;

namespace Btr.IntegrationTests;

public class RegistrationEndpointTests : IClassFixture<BtrWebApplicationFactory>
{
    private readonly BtrWebApplicationFactory _factory;

    public RegistrationEndpointTests(BtrWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Post_Registration_WithoutAuthToken_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/registrations", new
        {
            TournamentId = Guid.NewGuid(),
            PlayerName = "John Doe",
            Nickname = "Johnny",
            ContactInfo = "john@example.com"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Post_Registration_WithValidAuthAndPayload_ReturnsCreated()
    {
        using var client = _factory.CreateClient();

        // First, create a tournament
        var tournamentResponse = await client.PostAsJsonAsync("/api/v1/tournaments", new
        {
            Name = "Registration Test Tournament",
            Location = "Test Club",
            StartDate = new DateTime(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2026, 9, 3, 0, 0, 0, DateTimeKind.Utc)
        });

        Assert.Equal(HttpStatusCode.Created, tournamentResponse.StatusCode);
        var tournamentBody = await tournamentResponse.Content.ReadFromJsonAsync<TournamentCreatedResponse>();
        Assert.NotNull(tournamentBody);
        var tournamentId = tournamentBody.TournamentId;

        // Register a user and get a token
        var registerResponse = await client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            UsernameOrEmail = "testuser@example.com",
            Password = "SecurePassword123!"
        });

        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);

        // Login to get token
        var loginResponse = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            UsernameOrEmail = "testuser@example.com",
            Password = "SecurePassword123!"
        });

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var loginBody = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(loginBody);
        var token = loginBody.AccessToken;

        // Now post registration with token
        var authenticatedClient = _factory.CreateClient();
        authenticatedClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var registrationResponse = await authenticatedClient.PostAsJsonAsync("/api/v1/registrations", new
        {
            TournamentId = tournamentId,
            PlayerName = "Jane Smith",
            Nickname = "JS",
            ContactInfo = "jane@example.com"
        });

        Assert.Equal(HttpStatusCode.Created, registrationResponse.StatusCode);
        var body = await registrationResponse.Content.ReadFromJsonAsync<RegistrationCreatedResponse>();
        Assert.NotNull(body);
        Assert.NotEqual(Guid.Empty, body.RegistrationId);
        Assert.NotNull(registrationResponse.Headers.Location);
        Assert.Contains(body.RegistrationId.ToString(), registrationResponse.Headers.Location.ToString());
    }

    [Fact]
    public async Task Post_Registration_WithInvalidTournamentId_ReturnsBadRequest()
    {
        using var client = _factory.CreateClient();

        // Register and login to get token
        var testUsername = $"testuser{Guid.NewGuid():N}@example.com";
        var registerResponse = await client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            UsernameOrEmail = testUsername,
            Password = "SecurePassword123!"
        });
        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);

        var loginResponse = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            UsernameOrEmail = testUsername,
            Password = "SecurePassword123!"
        });
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var loginBody = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(loginBody);
        var token = loginBody.AccessToken;

        // Now post registration with invalid tournament ID
        var authenticatedClient = _factory.CreateClient();
        authenticatedClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var registrationResponse = await authenticatedClient.PostAsJsonAsync("/api/v1/registrations", new
        {
            TournamentId = Guid.NewGuid(),
            PlayerName = "Jane Smith",
            Nickname = "JS",
            ContactInfo = "jane@example.com"
        });

        Assert.Equal(HttpStatusCode.BadRequest, registrationResponse.StatusCode);
    }

    [Fact]
    public async Task Post_Registration_WithMissingPlayerName_ReturnsBadRequest()
    {
        using var client = _factory.CreateClient();

        // First, create a tournament
        var tournamentResponse = await client.PostAsJsonAsync("/api/v1/tournaments", new
        {
            Name = "Registration Test Tournament 2",
            Location = "Test Club",
            StartDate = new DateTime(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2026, 9, 3, 0, 0, 0, DateTimeKind.Utc)
        });

        Assert.Equal(HttpStatusCode.Created, tournamentResponse.StatusCode);
        var tournamentBody = await tournamentResponse.Content.ReadFromJsonAsync<TournamentCreatedResponse>();
        Assert.NotNull(tournamentBody);
        var tournamentId = tournamentBody.TournamentId;

        // Register and login
        var testUsername2 = $"testuser{Guid.NewGuid():N}@example.com";
        var registerResponse = await client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            UsernameOrEmail = testUsername2,
            Password = "SecurePassword123!"
        });
        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);

        var loginResponse = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            UsernameOrEmail = testUsername2,
            Password = "SecurePassword123!"
        });
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var loginBody = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(loginBody);
        var token = loginBody.AccessToken;

        // Post registration with missing PlayerName
        var authenticatedClient = _factory.CreateClient();
        authenticatedClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var registrationResponse = await authenticatedClient.PostAsJsonAsync("/api/v1/registrations", new
        {
            TournamentId = tournamentId,
            PlayerName = "",
            Nickname = "JS",
            ContactInfo = "jane@example.com"
        });

        Assert.Equal(HttpStatusCode.BadRequest, registrationResponse.StatusCode);
    }

    [Fact]
    public async Task Post_Registration_WithMissingContactInfo_ReturnsBadRequest()
    {
        using var client = _factory.CreateClient();

        // First, create a tournament
        var tournamentResponse = await client.PostAsJsonAsync("/api/v1/tournaments", new
        {
            Name = "Registration Test Tournament 3",
            Location = "Test Club",
            StartDate = new DateTime(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2026, 9, 3, 0, 0, 0, DateTimeKind.Utc)
        });

        Assert.Equal(HttpStatusCode.Created, tournamentResponse.StatusCode);
        var tournamentBody = await tournamentResponse.Content.ReadFromJsonAsync<TournamentCreatedResponse>();
        Assert.NotNull(tournamentBody);
        var tournamentId = tournamentBody.TournamentId;

        // Register and login
        var testUsername3 = $"testuser{Guid.NewGuid():N}@example.com";
        var registerResponse = await client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            UsernameOrEmail = testUsername3,
            Password = "SecurePassword123!"
        });
        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);

        var loginResponse = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            UsernameOrEmail = testUsername3,
            Password = "SecurePassword123!"
        });
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var loginBody = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(loginBody);
        var token = loginBody.AccessToken;

        // Post registration with missing ContactInfo
        var authenticatedClient = _factory.CreateClient();
        authenticatedClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var registrationResponse = await authenticatedClient.PostAsJsonAsync("/api/v1/registrations", new
        {
            TournamentId = tournamentId,
            PlayerName = "John Doe",
            Nickname = "JD",
            ContactInfo = ""
        });

        Assert.Equal(HttpStatusCode.BadRequest, registrationResponse.StatusCode);
    }

    [Fact]
    public async Task Get_Registrations_ReturnsOkAndCollection()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/registrations");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<List<RegistrationReadResponse>>();
        Assert.NotNull(body);
    }

    [Fact]
    public async Task Get_RegistrationById_WhenExists_ReturnsOk()
    {
        using var client = _factory.CreateClient();

        var createdRegistrationId = await CreateRegistrationAsync(client);

        var response = await client.GetAsync($"/api/v1/registrations/{createdRegistrationId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<RegistrationReadResponse>();
        Assert.NotNull(body);
        Assert.Equal(createdRegistrationId, body.Id);
    }

    [Fact]
    public async Task Get_RegistrationById_WhenMissing_ReturnsNotFound()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync($"/api/v1/registrations/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_RegistrationById_WhenIdFormatInvalid_ReturnsBadRequest()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/registrations/not-a-guid");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_Registration_WithoutAuthToken_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient();

        var response = await client.PutAsJsonAsync($"/api/v1/registrations/{Guid.NewGuid()}", new
        {
            TournamentId = Guid.NewGuid(),
            PlayerName = "Updated Name",
            Nickname = "UN",
            ContactInfo = "updated@example.com"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Put_Registration_WithValidPayload_ReturnsOkAndUpdatesData()
    {
        using var client = _factory.CreateClient();

        var registrationId = await CreateRegistrationAsync(client);

        var secondTournamentResponse = await client.PostAsJsonAsync("/api/v1/tournaments", new
        {
            Name = $"Update Test Tournament {Guid.NewGuid():N}",
            Location = "Test Club",
            StartDate = new DateTime(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2026, 10, 3, 0, 0, 0, DateTimeKind.Utc)
        });
        Assert.Equal(HttpStatusCode.Created, secondTournamentResponse.StatusCode);
        var secondTournament = await secondTournamentResponse.Content.ReadFromJsonAsync<TournamentCreatedResponse>();
        Assert.NotNull(secondTournament);

        var token = await CreateAndLoginUserAsync(client);

        using var authenticatedClient = _factory.CreateClient();
        authenticatedClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var updateResponse = await authenticatedClient.PutAsJsonAsync($"/api/v1/registrations/{registrationId}", new
        {
            TournamentId = secondTournament.TournamentId,
            PlayerName = "Updated Name",
            Nickname = "UpdatedNick",
            ContactInfo = "updated@example.com"
        });

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var getResponse = await client.GetAsync($"/api/v1/registrations/{registrationId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var updated = await getResponse.Content.ReadFromJsonAsync<RegistrationReadResponse>();
        Assert.NotNull(updated);
        Assert.Equal(secondTournament.TournamentId, updated.TournamentId);
        Assert.Equal("Updated Name", updated.PlayerName);
        Assert.Equal("UpdatedNick", updated.Nickname);
        Assert.Equal("updated@example.com", updated.ContactInfo);
    }

    [Fact]
    public async Task Put_Registration_WhenMissing_ReturnsNotFound()
    {
        using var client = _factory.CreateClient();
        var token = await CreateAndLoginUserAsync(client);

        var tournamentResponse = await client.PostAsJsonAsync("/api/v1/tournaments", new
        {
            Name = $"Missing Update Tournament {Guid.NewGuid():N}",
            Location = "Test Club",
            StartDate = new DateTime(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2026, 10, 3, 0, 0, 0, DateTimeKind.Utc)
        });
        var tournamentBody = await tournamentResponse.Content.ReadFromJsonAsync<TournamentCreatedResponse>();
        Assert.NotNull(tournamentBody);

        using var authenticatedClient = _factory.CreateClient();
        authenticatedClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await authenticatedClient.PutAsJsonAsync($"/api/v1/registrations/{Guid.NewGuid()}", new
        {
            TournamentId = tournamentBody.TournamentId,
            PlayerName = "Updated Name",
            Nickname = "UN",
            ContactInfo = "updated@example.com"
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Put_Registration_WithInvalidPayload_ReturnsBadRequest()
    {
        using var client = _factory.CreateClient();

        var registrationId = await CreateRegistrationAsync(client);
        var token = await CreateAndLoginUserAsync(client);

        using var authenticatedClient = _factory.CreateClient();
        authenticatedClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await authenticatedClient.PutAsJsonAsync($"/api/v1/registrations/{registrationId}", new
        {
            TournamentId = Guid.Empty,
            PlayerName = "",
            Nickname = "UN",
            ContactInfo = ""
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_Registration_WithInvalidIdFormat_ReturnsBadRequest()
    {
        using var client = _factory.CreateClient();
        var token = await CreateAndLoginUserAsync(client);

        using var authenticatedClient = _factory.CreateClient();
        authenticatedClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await authenticatedClient.PutAsJsonAsync("/api/v1/registrations/not-a-guid", new
        {
            TournamentId = Guid.NewGuid(),
            PlayerName = "Updated Name",
            Nickname = "UN",
            ContactInfo = "updated@example.com"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Registration_WithoutAuthToken_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient();

        var response = await client.DeleteAsync($"/api/v1/registrations/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Registration_WhenExists_ReturnsNoContent_AndThenNotFound()
    {
        using var client = _factory.CreateClient();

        var registrationId = await CreateRegistrationAsync(client);
        var token = await CreateAndLoginUserAsync(client);

        using var authenticatedClient = _factory.CreateClient();
        authenticatedClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var deleteResponse = await authenticatedClient.DeleteAsync($"/api/v1/registrations/{registrationId}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await client.GetAsync($"/api/v1/registrations/{registrationId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_Registration_WhenMissing_ReturnsNotFound()
    {
        using var client = _factory.CreateClient();
        var token = await CreateAndLoginUserAsync(client);

        using var authenticatedClient = _factory.CreateClient();
        authenticatedClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await authenticatedClient.DeleteAsync($"/api/v1/registrations/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Registration_WithInvalidIdFormat_ReturnsBadRequest()
    {
        using var client = _factory.CreateClient();
        var token = await CreateAndLoginUserAsync(client);

        using var authenticatedClient = _factory.CreateClient();
        authenticatedClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await authenticatedClient.DeleteAsync("/api/v1/registrations/not-a-guid");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task<string> CreateAndLoginUserAsync(HttpClient client)
    {
        var testUsername = $"testuser{Guid.NewGuid():N}@example.com";
        var registerResponse = await client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            UsernameOrEmail = testUsername,
            Password = "SecurePassword123!"
        });
        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);

        var loginResponse = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            UsernameOrEmail = testUsername,
            Password = "SecurePassword123!"
        });
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var loginBody = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(loginBody);
        return loginBody.AccessToken!;
    }

    private async Task<Guid> CreateRegistrationAsync(HttpClient client)
    {
        var tournamentResponse = await client.PostAsJsonAsync("/api/v1/tournaments", new
        {
            Name = $"Registration Test Tournament {Guid.NewGuid():N}",
            Location = "Test Club",
            StartDate = new DateTime(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2026, 9, 3, 0, 0, 0, DateTimeKind.Utc)
        });

        Assert.Equal(HttpStatusCode.Created, tournamentResponse.StatusCode);
        var tournamentBody = await tournamentResponse.Content.ReadFromJsonAsync<TournamentCreatedResponse>();
        Assert.NotNull(tournamentBody);

        var testUsername = $"testuser{Guid.NewGuid():N}@example.com";
        var registerResponse = await client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            UsernameOrEmail = testUsername,
            Password = "SecurePassword123!"
        });
        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);

        var loginResponse = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            UsernameOrEmail = testUsername,
            Password = "SecurePassword123!"
        });
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var loginBody = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(loginBody);

        using var authenticatedClient = _factory.CreateClient();
        authenticatedClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginBody.AccessToken);

        var registrationResponse = await authenticatedClient.PostAsJsonAsync("/api/v1/registrations", new
        {
            TournamentId = tournamentBody.TournamentId,
            PlayerName = "Jane Smith",
            Nickname = "JS",
            ContactInfo = "jane@example.com"
        });

        Assert.Equal(HttpStatusCode.Created, registrationResponse.StatusCode);
        var registrationBody = await registrationResponse.Content.ReadFromJsonAsync<RegistrationCreatedResponse>();
        Assert.NotNull(registrationBody);
        return registrationBody.RegistrationId;
    }

    private class TournamentCreatedResponse
    {
        public Guid TournamentId { get; set; }
    }

    private class RegistrationCreatedResponse
    {
        public Guid RegistrationId { get; set; }
    }

    private class RegistrationReadResponse
    {
        public Guid Id { get; set; }

        public Guid TournamentId { get; set; }

        public string? PlayerName { get; set; }

        public string? Nickname { get; set; }

        public string? ContactInfo { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }

    private class LoginResponse
    {
        public string? AccessToken { get; set; }
    }
}
