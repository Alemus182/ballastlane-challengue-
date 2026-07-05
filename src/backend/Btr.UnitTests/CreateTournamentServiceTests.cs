using Btr.Application.Abstractions.Persistence;
using Btr.Application.Features.Tournaments.Create;
using Btr.Domain.Entities;

namespace Btr.UnitTests;

public class CreateTournamentServiceTests
{
    [Fact]
    public async Task ExecuteAsync_WhenRequestIsValid_ShouldPersistTournamentAndReturnId()
    {
        var repository = new FakeTournamentRepository();
        var service = new CreateTournamentService(repository);

        var result = await service.ExecuteAsync(new CreateTournamentRequest(
            "Spring Open",
            "Billiard Club A",
            new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 8, 3, 0, 0, 0, DateTimeKind.Utc)));

        Assert.True(result.Success);
        Assert.NotNull(result.TournamentId);
        var saved = Assert.Single(repository.Tournaments);
        Assert.Equal("Spring Open", saved.Name);
        Assert.Equal("Billiard Club A", saved.Location);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNameIsBlank_ShouldReturnFailure()
    {
        var repository = new FakeTournamentRepository();
        var service = new CreateTournamentService(repository);

        var result = await service.ExecuteAsync(new CreateTournamentRequest(
            "   ",
            "Billiard Club A",
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(2)));

        Assert.False(result.Success);
        Assert.Equal("Name is required.", result.Error);
        Assert.Empty(repository.Tournaments);
    }

    [Fact]
    public async Task ExecuteAsync_WhenLocationIsBlank_ShouldReturnFailure()
    {
        var repository = new FakeTournamentRepository();
        var service = new CreateTournamentService(repository);

        var result = await service.ExecuteAsync(new CreateTournamentRequest(
            "Spring Open",
            "",
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(2)));

        Assert.False(result.Success);
        Assert.Equal("Location is required.", result.Error);
        Assert.Empty(repository.Tournaments);
    }

    [Fact]
    public async Task ExecuteAsync_WhenEndDateIsBeforeStartDate_ShouldReturnFailure()
    {
        var repository = new FakeTournamentRepository();
        var service = new CreateTournamentService(repository);

        var result = await service.ExecuteAsync(new CreateTournamentRequest(
            "Spring Open",
            "Billiard Club A",
            new DateTime(2026, 8, 3, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc)));

        Assert.False(result.Success);
        Assert.Equal("End date must be greater than or equal to start date.", result.Error);
        Assert.Empty(repository.Tournaments);
    }

    [Fact]
    public async Task ExecuteAsync_WhenStartDateIsDefault_ShouldReturnFailure()
    {
        var repository = new FakeTournamentRepository();
        var service = new CreateTournamentService(repository);

        var result = await service.ExecuteAsync(new CreateTournamentRequest(
            "Spring Open",
            "Billiard Club A",
            default,
            new DateTime(2026, 8, 3, 0, 0, 0, DateTimeKind.Utc)));

        Assert.False(result.Success);
        Assert.Equal("Start date is required.", result.Error);
        Assert.Empty(repository.Tournaments);
    }

    private sealed class FakeTournamentRepository : ITournamentRepository
    {
        public List<Tournament> Tournaments { get; } = [];

        public Task<IReadOnlyList<Tournament>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<Tournament>>(Tournaments);
        }

        public Task AddAsync(Tournament tournament, CancellationToken cancellationToken = default)
        {
            Tournaments.Add(tournament);
            return Task.CompletedTask;
        }
    }
}
