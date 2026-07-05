using Btr.Application.Abstractions.Persistence;
using Btr.Application.Abstractions.Security;
using Btr.Application.Features.Auth.Login;
using Btr.Domain.Entities;

namespace Btr.UnitTests;

public class LoginUserServiceTests
{
    [Fact]
    public async Task ExecuteAsync_WhenCredentialsAreValid_ShouldReturnToken()
    {
        var repository = new FakeUserRepository();
        var hasher = new FakePasswordHasher(isValid: true);
        var tokenGenerator = new FakeTokenGenerator();

        repository.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            UsernameOrEmail = "lemus@test.com",
            PasswordHash = "hashed::P@ssw0rd",
            CreatedAtUtc = DateTime.UtcNow
        });

        var service = new LoginUserService(repository, hasher, tokenGenerator);

        var result = await service.ExecuteAsync(new LoginUserRequest("lemus@test.com", "P@ssw0rd"));

        Assert.True(result.Success);
        Assert.Equal("fake.jwt.token", result.AccessToken);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCredentialsAreInvalid_ShouldReturnFailure()
    {
        var repository = new FakeUserRepository();
        var hasher = new FakePasswordHasher(isValid: false);
        var tokenGenerator = new FakeTokenGenerator();

        repository.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            UsernameOrEmail = "lemus@test.com",
            PasswordHash = "hashed::P@ssw0rd",
            CreatedAtUtc = DateTime.UtcNow
        });

        var service = new LoginUserService(repository, hasher, tokenGenerator);

        var result = await service.ExecuteAsync(new LoginUserRequest("lemus@test.com", "wrong-password"));

        Assert.False(result.Success);
        Assert.Equal("Invalid credentials.", result.Error);
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

    private sealed class FakePasswordHasher(bool isValid) : IPasswordHasher
    {
        public string HashPassword(string password) => $"hashed::{password}";

        public bool VerifyPassword(string password, string passwordHash) => isValid;
    }

    private sealed class FakeTokenGenerator : ITokenGenerator
    {
        public string GenerateToken(User user) => "fake.jwt.token";
    }
}
