using Btr.Application.Abstractions.Persistence;
using Btr.Application.Features.Registrations.Delete;
using Btr.Domain.Entities;

namespace Btr.UnitTests;

public class DeleteRegistrationServiceTests
{
    [Fact]
    public async Task ExecuteAsync_WhenRegistrationExists_DeletesRegistration()
    {
        var repository = new FakeRegistrationRepository();
        var registration = new Registration
        {
            Id = Guid.NewGuid(),
            TournamentId = Guid.NewGuid(),
            PlayerName = "Jane Doe",
            ContactInfo = "jane@example.com",
            CreatedByUserId = Guid.NewGuid(),
            CreatedAtUtc = DateTime.UtcNow
        };
        repository.Registrations.Add(registration);

        var service = new DeleteRegistrationService(repository);
        var result = await service.ExecuteAsync(registration.Id);

        Assert.True(result.Success);
        Assert.Empty(repository.Registrations);
    }

    [Fact]
    public async Task ExecuteAsync_WhenRegistrationMissing_ReturnsFailure()
    {
        var repository = new FakeRegistrationRepository();
        var service = new DeleteRegistrationService(repository);

        var result = await service.ExecuteAsync(Guid.NewGuid());

        Assert.False(result.Success);
        Assert.Equal("Registration not found.", result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_WhenRegistrationIdEmpty_ReturnsFailure()
    {
        var repository = new FakeRegistrationRepository();
        var service = new DeleteRegistrationService(repository);

        var result = await service.ExecuteAsync(Guid.Empty);

        Assert.False(result.Success);
        Assert.Equal("RegistrationId is required.", result.Error);
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
