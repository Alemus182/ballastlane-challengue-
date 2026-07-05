using Btr.Application.Abstractions.Persistence;

namespace Btr.Application.Features.Tournaments.GetList;

public sealed class GetTournamentsService
{
    private readonly ITournamentRepository _tournamentRepository;

    public GetTournamentsService(ITournamentRepository tournamentRepository)
    {
        _tournamentRepository = tournamentRepository;
    }

    public async Task<GetTournamentsResult> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var tournaments = await _tournamentRepository.GetAllAsync(cancellationToken);

        var dtos = tournaments
            .Select(t => new TournamentDto(t.Id, t.Name, t.Location, t.StartDate, t.EndDate))
            .ToList();

        return GetTournamentsResult.Ok(dtos);
    }
}
