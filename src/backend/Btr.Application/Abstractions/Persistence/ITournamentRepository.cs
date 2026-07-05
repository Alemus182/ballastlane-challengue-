using Btr.Domain.Entities;

namespace Btr.Application.Abstractions.Persistence;

public interface ITournamentRepository
{
    Task<IReadOnlyList<Tournament>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(Tournament tournament, CancellationToken cancellationToken = default);
}
