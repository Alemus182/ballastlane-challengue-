using Btr.Application.Abstractions.Persistence;
using Btr.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Btr.Infrastructure.Persistence.Repositories;

public class RegistrationRepository : IRegistrationRepository
{
    private readonly AppDbContext _dbContext;

    public RegistrationRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Registration>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Registrations
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<Registration?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Registrations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddAsync(Registration registration, CancellationToken cancellationToken = default)
    {
        await _dbContext.Registrations.AddAsync(registration, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Registration registration, CancellationToken cancellationToken = default)
    {
        _dbContext.Registrations.Update(registration);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Registration registration, CancellationToken cancellationToken = default)
    {
        _dbContext.Registrations.Remove(registration);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
