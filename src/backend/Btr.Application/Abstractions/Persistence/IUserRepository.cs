using Btr.Domain.Entities;

namespace Btr.Application.Abstractions.Persistence;

public interface IUserRepository
{
    Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail, CancellationToken cancellationToken = default);

    Task AddAsync(User user, CancellationToken cancellationToken = default);
}
