namespace Btr.Application.Features.Tournaments.GetList;

public sealed record TournamentDto(Guid Id, string Name, string Location, DateTime StartDate, DateTime EndDate);

public sealed class GetTournamentsResult
{
    public bool Success { get; init; }

    public IReadOnlyList<TournamentDto> Tournaments { get; init; } = [];

    public string? Error { get; init; }

    public static GetTournamentsResult Ok(IReadOnlyList<TournamentDto> items) =>
        new() { Success = true, Tournaments = items };

    public static GetTournamentsResult Fail(string error) =>
        new() { Success = false, Error = error };
}
