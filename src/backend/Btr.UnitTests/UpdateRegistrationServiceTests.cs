using Btr.Application.Abstractions.Persistence;
using Btr.Application.Features.Registrations.Update;
using Btr.Domain.Entities;

namespace Btr.UnitTests;

public class UpdateRegistrationServiceTests
{
    [Fact]
    public async Task ExecuteAsync_WhenRequestIsValid_UpdatesRegistration()
    {
        var tournamentRepository = new FakeTournamentRepository();
        var registrationRepository = new FakeRegistrationRepository();
        var service = new UpdateRegistrationService(registrationRepository, tournamentRepository);

        var originalTournament = new Tournament
        {
            Id = Guid.NewGuid(),
            Name = "Old Tournament",
            Location = "Club A",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1)
        };
        var newTournament = new Tournament
        {
            Id = Guid.NewGuid(),
            Name = "New Tournament",
            Location = "Club B",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(2)
        };
        tournamentRepository.Tournaments.Add(originalTournament);
        tournamentRepository.Tournaments.Add(newTournament);

        var registration = new Registration
        {
            Id = Guid.NewGuid(),
            TournamentId = originalTournament.Id,
            PlayerName = "Old Name",
            Nickname = "Old",
            ContactInfo = "old@example.com",
            CreatedByUserId = Guid.NewGuid(),
            CreatedAtUtc = DateTime.UtcNow
        };
        registrationRepository.Registrations.Add(registration);

        var result = await service.ExecuteAsync(
            registration.Id,
            new UpdateRegistrationRequest(newTournament.Id, "New Name", "NewNick", "new@example.com"));

        Assert.True(result.Success);
        var updated = Assert.Single(registrationRepository.Registrations);
        Assert.Equal(newTournament.Id, updated.TournamentId);
        Assert.Equal("New Name", updated.PlayerName);
        Assert.Equal("NewNick", updated.Nickname);
        Assert.Equal("new@example.com", updated.ContactInfo);
    }

    [Fact]
    public async Task ExecuteAsync_WhenRegistrationMissing_ReturnsFailure()
    {
        var tournamentRepository = new FakeTournamentRepository();
        var registrationRepository = new FakeRegistrationRepository();
        var service = new UpdateRegistrationService(registrationRepository, tournamentRepository);

        var result = await service.ExecuteAsync(
            Guid.NewGuid(),
            new UpdateRegistrationRequest(Guid.NewGuid(), "Name", null, "contact@example.com"));

        Assert.False(result.Success);
        Assert.Equal("Registration not found.", result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTournamentMissing_ReturnsFailure()
    {
        var tournamentRepository = new FakeTournamentRepository();
        var registrationRepository = new FakeRegistrationRepository();
        var service = new UpdateRegistrationService(registrationRepository, tournamentRepository);

        registrationRepository.Registrations.Add(new Registration
        {
            Id = Guid.NewGuid(),
            TournamentId = Guid.NewGuid(),
            PlayerName = "Name",
            ContactInfo = "contact@example.com",
            CreatedByUserId = Guid.NewGuid(),
            CreatedAtUtc = DateTime.UtcNow
        });

        var result = await service.ExecuteAsync(
            registrationRepository.Registrations[0].Id,
            new UpdateRegistrationRequest(Guid.NewGuid(), "Name", null, "contact@example.com"));

        Assert.False(result.Success);
        Assert.Equal("Tournament not found.", result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPlayerNameBlank_ReturnsFailure()
    {
        var tournamentRepository = new FakeTournamentRepository();
        var registrationRepository = new FakeRegistrationRepository();
        var service = new UpdateRegistrationService(registrationRepository, tournamentRepository);

        var result = await service.ExecuteAsync(
            Guid.NewGuid(),
            new UpdateRegistrationRequest(Guid.NewGuid(), " ", null, "contact@example.com"));

        Assert.False(result.Success);
        Assert.Equal("PlayerName is required.", result.Error);
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
