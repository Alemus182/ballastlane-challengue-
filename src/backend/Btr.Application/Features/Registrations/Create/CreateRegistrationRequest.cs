namespace Btr.Application.Features.Registrations.Create;

public sealed record CreateRegistrationRequest(
    Guid TournamentId,
    string PlayerName,
    string? Nickname,
    string ContactInfo
);
