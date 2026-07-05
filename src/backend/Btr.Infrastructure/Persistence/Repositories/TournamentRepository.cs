using Btr.Application.Abstractions.Persistence;
using Btr.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Btr.Infrastructure.Persistence.Repositories;

public class TournamentRepository : ITournamentRepository
{
    private readonly AppDbContext _dbContext;

    public TournamentRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Tournament>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Tournaments
            .OrderBy(x => x.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Tournament tournament, CancellationToken cancellationToken = default)
    {
        await _dbContext.Tournaments.AddAsync(tournament, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
