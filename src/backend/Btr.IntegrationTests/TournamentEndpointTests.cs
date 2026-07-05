using System.Net;
using System.Net.Http.Json;

namespace Btr.IntegrationTests;

public class TournamentEndpointTests : IClassFixture<BtrWebApplicationFactory>
{
    private readonly BtrWebApplicationFactory _factory;

    public TournamentEndpointTests(BtrWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Post_Tournament_WithValidPayload_ReturnsCreated()
    {
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/tournaments", new
        {
            Name = "Spring Open",
            Location = "Billiard Club A",
            StartDate = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2026, 8, 3, 0, 0, 0, DateTimeKind.Utc)
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<TournamentCreatedResponse>();
        Assert.NotNull(body);
        Assert.NotEqual(Guid.Empty, body.TournamentId);
    }

    [Fact]
    public async Task Post_Tournament_WithMissingName_ReturnsBadRequest()
    {
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/tournaments", new
        {
            Name = "",
            Location = "Billiard Club A",
            StartDate = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2026, 8, 3, 0, 0, 0, DateTimeKind.Utc)
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Get_Tournaments_ReturnsOkWithList()
    {
        using var client = _factory.CreateClient();

        // Create one first so the list is not empty
        var createResponse = await client.PostAsJsonAsync("/api/v1/tournaments", new
        {
            Name = $"List Test {Guid.NewGuid():N}",
            Location = "Club B",
            StartDate = new DateTime(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2026, 9, 2, 0, 0, 0, DateTimeKind.Utc)
        });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var response = await client.GetAsync("/api/v1/tournaments");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var list = await response.Content.ReadFromJsonAsync<List<TournamentListItemResponse>>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
        Assert.Contains(list, x => x.Name.StartsWith("List Test ", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Post_Tournament_WithMissingDates_ReturnsBadRequest()
    {
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/tournaments", new
        {
            Name = "Missing Dates Tournament",
            Location = "Billiard Club A"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_Tournament_WithEndDateBeforeStartDate_ReturnsBadRequest()
    {
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/tournaments", new
        {
            Name = "Invalid Range Tournament",
            Location = "Billiard Club A",
            StartDate = new DateTime(2026, 8, 3, 0, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private sealed record TournamentCreatedResponse(Guid TournamentId);
    private sealed record TournamentListItemResponse(Guid Id, string Name, string Location, DateTime StartDate, DateTime EndDate);
}
