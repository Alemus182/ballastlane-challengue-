namespace Btr.Application.Features.Tournaments.Create;

public sealed record CreateTournamentRequest(string Name, string Location, DateTime StartDate, DateTime EndDate);
