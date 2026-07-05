using Btr.Application.Abstractions.Persistence;
using Btr.Domain.Entities;

namespace Btr.Application.Features.Tournaments.Create;

public sealed class CreateTournamentService
{
    private readonly ITournamentRepository _tournamentRepository;

    public CreateTournamentService(ITournamentRepository tournamentRepository)
    {
        _tournamentRepository = tournamentRepository;
    }

    public async Task<CreateTournamentResult> ExecuteAsync(CreateTournamentRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return CreateTournamentResult.Fail("Name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Location))
        {
            return CreateTournamentResult.Fail("Location is required.");
        }

        if (request.StartDate == default)
        {
            return CreateTournamentResult.Fail("Start date is required.");
        }

        if (request.EndDate == default)
        {
            return CreateTournamentResult.Fail("End date is required.");
        }

        if (request.EndDate < request.StartDate)
        {
            return CreateTournamentResult.Fail("End date must be greater than or equal to start date.");
        }

        var tournament = new Tournament
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Location = request.Location.Trim(),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
        };

        await _tournamentRepository.AddAsync(tournament, cancellationToken);
        return CreateTournamentResult.Ok(tournament.Id);
    }
}
