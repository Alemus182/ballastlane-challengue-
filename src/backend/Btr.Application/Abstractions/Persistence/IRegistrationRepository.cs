using Btr.Domain.Entities;

namespace Btr.Application.Abstractions.Persistence;

public interface IRegistrationRepository
{
    Task<IReadOnlyList<Registration>> ListAsync(CancellationToken cancellationToken = default);

    Task<Registration?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(Registration registration, CancellationToken cancellationToken = default);

    Task UpdateAsync(Registration registration, CancellationToken cancellationToken = default);

    Task DeleteAsync(Registration registration, CancellationToken cancellationToken = default);
}
