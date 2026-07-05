using Btr.Application.Abstractions.Persistence;
using Btr.Application.Features.Registrations.Get;
using Btr.Domain.Entities;

namespace Btr.UnitTests;

public class GetRegistrationQueriesTests
{
    [Fact]
    public async Task GetRegistrationsService_WhenRepositoryHasData_ReturnsMappedList()
    {
        var repository = new FakeRegistrationRepository();
        var registration = CreateRegistration();
        repository.Registrations.Add(registration);

        var service = new GetRegistrationsService(repository);
        var result = await service.ExecuteAsync();

        Assert.True(result.Success);
        var item = Assert.Single(result.Registrations);
        Assert.Equal(registration.Id, item.Id);
        Assert.Equal(registration.TournamentId, item.TournamentId);
        Assert.Equal(registration.PlayerName, item.PlayerName);
        Assert.Equal(registration.Nickname, item.Nickname);
        Assert.Equal(registration.ContactInfo, item.ContactInfo);
        Assert.Equal(registration.CreatedAtUtc, item.CreatedAtUtc);
    }

    [Fact]
    public async Task GetRegistrationsService_WhenRepositoryIsEmpty_ReturnsEmptyList()
    {
        var repository = new FakeRegistrationRepository();
        var service = new GetRegistrationsService(repository);

        var result = await service.ExecuteAsync();

        Assert.True(result.Success);
        Assert.Empty(result.Registrations);
    }

    [Fact]
    public async Task GetRegistrationByIdService_WhenRegistrationExists_ReturnsSuccess()
    {
        var repository = new FakeRegistrationRepository();
        var registration = CreateRegistration();
        repository.Registrations.Add(registration);

        var service = new GetRegistrationByIdService(repository);
        var result = await service.ExecuteAsync(registration.Id);

        Assert.True(result.Success);
        Assert.NotNull(result.Registration);
        Assert.Equal(registration.Id, result.Registration!.Id);
    }

    [Fact]
    public async Task GetRegistrationByIdService_WhenRegistrationDoesNotExist_ReturnsFailure()
    {
        var repository = new FakeRegistrationRepository();
        var service = new GetRegistrationByIdService(repository);

        var result = await service.ExecuteAsync(Guid.NewGuid());

        Assert.False(result.Success);
        Assert.Equal("Registration not found.", result.Error);
        Assert.Null(result.Registration);
    }

    private static Registration CreateRegistration()
    {
        return new Registration
        {
            Id = Guid.NewGuid(),
            TournamentId = Guid.NewGuid(),
            PlayerName = "Jane Doe",
            Nickname = "JD",
            ContactInfo = "jane@example.com",
            CreatedByUserId = Guid.NewGuid(),
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    private sealed class FakeRegistrationRepository : IRegistrationRepository
    {
        public List<Registration> Registrations { get; } = [];

        public Task<IReadOnlyList<Registration>> ListAsync(CancellationToken cancellationToken = default)
        {
            var ordered = Registrations
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToList();
            return Task.FromResult<IReadOnlyList<Registration>>(ordered);
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
