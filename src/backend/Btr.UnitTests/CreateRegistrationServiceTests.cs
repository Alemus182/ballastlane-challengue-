using Btr.Application.Abstractions.Persistence;
using Btr.Application.Features.Registrations.Create;
using Btr.Domain.Entities;

namespace Btr.UnitTests;

public class CreateRegistrationServiceTests
{
    [Fact]
    public async Task ExecuteAsync_WhenRequestIsValid_ShouldPersistRegistrationAndReturnId()
    {
        var tournamentRepository = new FakeTournamentRepository();
        var registrationRepository = new FakeRegistrationRepository();
        var service = new CreateRegistrationService(registrationRepository, tournamentRepository);

        // First add a tournament so it exists
        var tournament = new Tournament
        {
            Id = Guid.NewGuid(),
            Name = "Test Tournament",
            Location = "Test Location",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(2)
        };
        tournamentRepository.Tournaments.Add(tournament);

        var userId = Guid.NewGuid();
        var result = await service.ExecuteAsync(new CreateRegistrationRequest(
            tournament.Id,
            "John Doe",
            "Johnny",
            "john@example.com"),
            userId);

        Assert.True(result.Success);
        Assert.NotNull(result.RegistrationId);
        Assert.NotEqual(Guid.Empty, result.RegistrationId);
        var saved = Assert.Single(registrationRepository.Registrations);
        Assert.Equal("John Doe", saved.PlayerName);
        Assert.Equal("Johnny", saved.Nickname);
        Assert.Equal("john@example.com", saved.ContactInfo);
        Assert.Equal(tournament.Id, saved.TournamentId);
        Assert.Equal(userId, saved.CreatedByUserId);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPlayerNameIsBlank_ShouldReturnFailure()
    {
        var tournamentRepository = new FakeTournamentRepository();
        var registrationRepository = new FakeRegistrationRepository();
        var service = new CreateRegistrationService(registrationRepository, tournamentRepository);

        var tournament = new Tournament
        {
            Id = Guid.NewGuid(),
            Name = "Test Tournament",
            Location = "Test Location",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(2)
        };
        tournamentRepository.Tournaments.Add(tournament);

        var result = await service.ExecuteAsync(new CreateRegistrationRequest(
            tournament.Id,
            "   ",
            null,
            "john@example.com"),
            Guid.NewGuid());

        Assert.False(result.Success);
        Assert.Equal("PlayerName is required.", result.Error);
        Assert.Empty(registrationRepository.Registrations);
    }

    [Fact]
    public async Task ExecuteAsync_WhenContactInfoIsBlank_ShouldReturnFailure()
    {
        var tournamentRepository = new FakeTournamentRepository();
        var registrationRepository = new FakeRegistrationRepository();
        var service = new CreateRegistrationService(registrationRepository, tournamentRepository);

        var tournament = new Tournament
        {
            Id = Guid.NewGuid(),
            Name = "Test Tournament",
            Location = "Test Location",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(2)
        };
        tournamentRepository.Tournaments.Add(tournament);

        var result = await service.ExecuteAsync(new CreateRegistrationRequest(
            tournament.Id,
            "John Doe",
            null,
            ""),
            Guid.NewGuid());

        Assert.False(result.Success);
        Assert.Equal("ContactInfo is required.", result.Error);
        Assert.Empty(registrationRepository.Registrations);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTournamentDoesNotExist_ShouldReturnFailure()
    {
        var tournamentRepository = new FakeTournamentRepository();
        var registrationRepository = new FakeRegistrationRepository();
        var service = new CreateRegistrationService(registrationRepository, tournamentRepository);

        var result = await service.ExecuteAsync(new CreateRegistrationRequest(
            Guid.NewGuid(),
            "John Doe",
            null,
            "john@example.com"),
            Guid.NewGuid());

        Assert.False(result.Success);
        Assert.Equal("Tournament not found.", result.Error);
        Assert.Empty(registrationRepository.Registrations);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTournamentIdIsEmpty_ShouldReturnFailure()
    {
        var tournamentRepository = new FakeTournamentRepository();
        var registrationRepository = new FakeRegistrationRepository();
        var service = new CreateRegistrationService(registrationRepository, tournamentRepository);

        var result = await service.ExecuteAsync(new CreateRegistrationRequest(
            Guid.Empty,
            "John Doe",
            null,
            "john@example.com"),
            Guid.NewGuid());

        Assert.False(result.Success);
        Assert.Equal("TournamentId is required.", result.Error);
        Assert.Empty(registrationRepository.Registrations);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCreatedByUserIdIsEmpty_ShouldReturnFailure()
    {
        var tournamentRepository = new FakeTournamentRepository();
        var registrationRepository = new FakeRegistrationRepository();
        var service = new CreateRegistrationService(registrationRepository, tournamentRepository);

        var tournament = new Tournament
        {
            Id = Guid.NewGuid(),
            Name = "Test Tournament",
            Location = "Test Location",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(2)
        };
        tournamentRepository.Tournaments.Add(tournament);

        var result = await service.ExecuteAsync(
            new CreateRegistrationRequest(
                tournament.Id,
                "John Doe",
                null,
                "john@example.com"),
            Guid.Empty);

        Assert.False(result.Success);
        Assert.Equal("CreatedByUserId is required.", result.Error);
        Assert.Empty(registrationRepository.Registrations);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldSetCreatedAtUtcToNearCurrentUtcTime()
    {
        var tournamentRepository = new FakeTournamentRepository();
        var registrationRepository = new FakeRegistrationRepository();
        var service = new CreateRegistrationService(registrationRepository, tournamentRepository);

        var tournament = new Tournament
        {
            Id = Guid.NewGuid(),
            Name = "Test Tournament",
            Location = "Test Location",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(2)
        };
        tournamentRepository.Tournaments.Add(tournament);

        var beforeCall = DateTime.UtcNow;
        var userId = Guid.NewGuid();
        var result = await service.ExecuteAsync(new CreateRegistrationRequest(
            tournament.Id,
            "John Doe",
            null,
            "john@example.com"),
            userId);
        var afterCall = DateTime.UtcNow;

        Assert.True(result.Success);
        var saved = Assert.Single(registrationRepository.Registrations);
        Assert.True(saved.CreatedAtUtc >= beforeCall && saved.CreatedAtUtc <= afterCall);
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

    private sealed class FakeRegistrationRepository : IRegistrationRepository
    {
        public List<Registration> Registrations { get; } = [];

        public Task<IReadOnlyList<Registration>> ListAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<Registration>>(Registrations);
        }

        public Task<Registration?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Registrations.FirstOrDefault(x => x.Id == id));
        }

        public Task AddAsync(Registration registration, CancellationToken cancellationToken = default)
        {
            Registrations.Add(registration);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Registration registration, CancellationToken cancellationToken = default)
        {
            var index = Registrations.FindIndex(x => x.Id == registration.Id);
            if (index >= 0)
            {
                Registrations[index] = registration;
            }

            return Task.CompletedTask;
        }

        public Task DeleteAsync(Registration registration, CancellationToken cancellationToken = default)
        {
            Registrations.RemoveAll(x => x.Id == registration.Id);
            return Task.CompletedTask;
        }
    }
}
