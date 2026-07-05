namespace Btr.Application.Features.Tournaments.Create;

public sealed class CreateTournamentResult
{
    public bool Success { get; init; }

    public Guid? TournamentId { get; init; }

    public string? Error { get; init; }

    public static CreateTournamentResult Ok(Guid id) =>
        new() { Success = true, TournamentId = id };

    public static CreateTournamentResult Fail(string error) =>
        new() { Success = false, Error = error };
}
