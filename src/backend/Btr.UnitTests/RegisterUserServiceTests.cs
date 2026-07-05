using Btr.Application.Abstractions.Persistence;
using Btr.Application.Abstractions.Security;
using Btr.Application.Features.Auth.Register;
using Btr.Domain.Entities;

namespace Btr.UnitTests;

public class RegisterUserServiceTests
{
    [Fact]
    public async Task ExecuteAsync_WhenUserIsNew_ShouldPersistHashedPassword()
    {
        var repository = new FakeUserRepository();
        var hasher = new FakePasswordHasher();
        var service = new RegisterUserService(repository, hasher);

        var result = await service.ExecuteAsync(new RegisterUserRequest("lemus@test.com", "P@ssw0rd"));

        Assert.True(result.Success);
        var createdUser = Assert.Single(repository.Users);
        Assert.Equal("lemus@test.com", createdUser.UsernameOrEmail);
        Assert.Equal("hashed::P@ssw0rd", createdUser.PasswordHash);
        Assert.NotEqual("P@ssw0rd", createdUser.PasswordHash);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserAlreadyExists_ShouldReturnFailure()
    {
        var repository = new FakeUserRepository();
        repository.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            UsernameOrEmail = "lemus@test.com",
            PasswordHash = "existing",
            CreatedAtUtc = DateTime.UtcNow
        });

        var hasher = new FakePasswordHasher();
        var service = new RegisterUserService(repository, hasher);

        var result = await service.ExecuteAsync(new RegisterUserRequest("lemus@test.com", "P@ssw0rd"));

        Assert.False(result.Success);
        Assert.Equal("User already exists.", result.Error);
        Assert.Single(repository.Users);
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        public List<User> Users { get; } = [];

        public Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail, CancellationToken cancellationToken = default)
        {
            var user = Users.FirstOrDefault(x => x.UsernameOrEmail == usernameOrEmail);
            return Task.FromResult(user);
        }

        public Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            Users.Add(user);
            return Task.CompletedTask;
        }
    }

    private sealed class FakePasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password) => $"hashed::{password}";

        public bool VerifyPassword(string password, string passwordHash) => true;
    }
}
